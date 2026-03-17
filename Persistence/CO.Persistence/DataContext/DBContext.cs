using CO.Domain.Contracts.Interfaces.Common;
using CO.Domain.Entities;
using CO.Persistence.Seeds;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CO.Persistence.DataContext;

public class DBContext(
    DbContextOptions<DBContext> options,
    ILogger<DBContext>? logger = null,
    IPublisher? publisher = null) : DbContext(options)
{
    private readonly ILogger<DBContext>? _logger = logger;
    private readonly IPublisher? _publisher = publisher;

    #region Sets

    public DbSet<Client> Clients { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<StaffMember> StaffMembers { get; set; }


    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                //var propertyAccess = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));

                var property = Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    [typeof(bool)],
                    parameter,
                    Expression.Constant(nameof(BaseEntity.IsDeleted)));

                var comparison = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(comparison, parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }

            if (typeof(ICursorPaginable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasKey(nameof(ICursorPaginable.Id));

                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex(nameof(ICursorPaginable.CreatedAt), nameof(ICursorPaginable.Id))
                    .HasDatabaseName($"IX_{entityType.GetTableName()}_CreatedAt_Id");
            }
        }

        // Seed Staff Members
        modelBuilder.Entity<StaffMember>().HasData(StaffMemberSeed.GetSeedData());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DBContext).Assembly);


    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            UpdateAuditFields();

            // Get all entities that have pending events
            var domainEntities = ChangeTracker.Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToList();

            var domainEvents = domainEntities.SelectMany(x => x.DomainEvents).ToList();

            // Clear events so they don't fire again on next save
            domainEntities.ForEach(x => x.ClearDomainEvents());

            var result = await base.SaveChangesAsync(ct);

            if (_publisher is not null)
            {
                try
                {
                    // Publish the events to MediatR
                    foreach (var domainEvent in domainEvents)
                    {
                        await _publisher.Publish(domainEvent, ct);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error occurred while publishing domain events");
                    throw;
                }
            }

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Log which entities were involved
            foreach (var entry in ex.Entries)
            {
                var entryName = entry.Entity.GetType().Name;
                _logger?.LogError("Concurrency conflict on: {EntryName}", entryName);
                _logger?.LogError("Entity State: {EntityState}", entry.State);
                _logger?.LogError("Entity: {Entity}", JsonSerializer.Serialize(entry.Entity));

                // Get current database values
                var databaseValues = await entry.GetDatabaseValuesAsync(ct);
                if (databaseValues == null)
                {
                    _logger?.LogInformation("Entity has been deleted from database");
                }
                else
                {
                    _logger?.LogInformation("Database values: {DatabaseValues}", JsonSerializer.Serialize(databaseValues.ToObject()));
                }
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger?.LogError(ex, "Database update failed. Exception: {ErrorMessage}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An unexpected error occurred. Exception: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var now = DateTimeOffset.UtcNow;
        var currentUserId = GetCurrentUserId();

        foreach (var entry in entries)
        {
            // Handle Automatic Tenant Stamping
            if (entry.State == EntityState.Added && entry.Entity is BaseEntity baseEntity)
            {
                
            }

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                entry.Entity.CreatedBy = currentUserId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                entry.Entity.UpdatedBy = currentUserId;
            }
        }
    }

    private string? GetCurrentUserId()
    {
        return Guid.CreateVersion7().ToString();
    }

    private static async Task TryRollbackAsync(DbTransaction transaction, CancellationToken ct)
    {
        try
        {
            // Check if transaction is still active
            if (transaction.Connection != null && transaction.Connection.State == ConnectionState.Open)
            {
                await transaction.RollbackAsync(ct);
            }
        }
        catch (InvalidOperationException)
        {
            // Transaction was already completed (committed/rolled back)
            // No action needed
        }
    }

    private static async Task DisposeTransactionSilentlyAsync(DbTransaction transaction)
    {
        try
        {
            await transaction.DisposeAsync();
        }
        catch
        {
            // Suppress disposal errors
            // This is a silent disposal, so we don't log or throw
        }
    }





}

