using CO.Domain.Contracts.Interfaces.Common;
using CO.Domain.Contracts.Interfaces.Repositories;
using CO.Domain.Contracts.Specifications;
using CO.Persistence.DataContext;
using CO.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CO.Persistence.Contracts.Implementations.Repositories;

public class Repository<T>(DBContext context) : IRepository<T> where T : class
{
    private readonly DBContext _context = context;

    public async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        await _context.Set<T>().AddAsync(entity, ct);
        return entity;
    }

    public virtual async Task<int> CountAsync(CancellationToken ct = default) => await _context.Set<T>().CountAsync(ct);

    public virtual async Task<int> CountAsync<TCursor>(ISpecification<T, TCursor> spec, CancellationToken ct = default)
    {
        var query = _context.Set<T>().AsNoTracking();

        // Apply filters only — no ordering, no take, no includes
        if (spec.Criteria != null)
            query = spec.Criteria.Aggregate(query, (current, criteria) => current.Where(criteria));

        // Cursor filter is intentionally excluded — count should reflect total matching records, not records after the current page position

        return await query.CountAsync(ct);
    }

    public async Task<T> DeleteAsync(Guid Id, CancellationToken ct = default)
    {
        var entity = await FindByIdAsync(Id, ct);

        if (entity == null)
            throw new Exception($"Entity with id {Id} not found");

        _context.Set<T>().Remove(entity);
        return entity;
    }

    public async Task<T> SoftDeleteAsync(Guid Id, CancellationToken ct = default)
    {
        var entity = await FindByIdAsync(Id, ct) ?? throw new Exception($"Entity with id {Id} not found");
        if (entity is ISoftDelete softDeletableEntity) 
        {
            softDeletableEntity.IsDeleted = true;
            _context.Set<T>().Update(entity); 
        }
        else
        {
            throw new NotSupportedException($"Entity type {typeof(T).Name} does not support this method.");
        }
        return entity;
    }

    public async Task<T> DeleteAsync(string Id, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(Id))
            throw new ArgumentException("Id cannot be null or empty", nameof(Id));
        var entity = await FindByIdAsync(Guid.Parse(Id), ct);
        if (entity == null)
            throw new Exception($"Entity with id {Id} not found");
        _context.Set<T>().Remove(entity);
        return entity;
    }
    
    public async Task<T> SoftDeleteAsync(string Id, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(Id))
            throw new ArgumentException("Id cannot be null or empty", nameof(Id));

        Guid guidId = Guid.Parse(Id); 

        var entity = await FindByIdAsync(guidId, ct) ?? throw new Exception($"Entity with id {Id} not found");
        if (entity is ISoftDelete softDeletableEntity) 
        {
            softDeletableEntity.IsDeleted = true;
            _context.Set<T>().Update(entity); 
        }
        else
        {
            throw new NotSupportedException($"Entity type {typeof(T).Name} does not support this method");
        }
        return entity;
    }

    public IQueryable<T> FindAll()
    {
        return _context.Set<T>().AsNoTracking();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression).AsNoTracking();

    }

    public async Task<T?> FindByIdAsync(Guid Id, CancellationToken ct = default)
    {
        return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == Id, ct);

    }

    /// <summary>
    /// Loads an entity with change tracking enabled. Use this before calling
    /// <see cref="UpdateAsync"/> or <see cref="DeleteAsync"/> when you need
    /// EF Core to detect property-level changes rather than marking all
    /// columns modified.
    /// </summary>
    public async Task<T?> FindByIdTrackedAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<T>().AsTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, ct);
    }

    /// <summary>
    /// Returns a tracked queryable for mutation operations (insert/update/delete).
    /// Use <see cref="FindAll"/> or <see cref="FindByCondition"/> for read-only queries.
    /// </summary>
    public IQueryable<T> FindByConditionTracked(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().AsTracking().Where(expression);
    }

    public async Task<List<T>> SearchAsync<TCursor>(ISpecification<T, TCursor> spec, CancellationToken ct = default)
    {
        return await _context.Set<T>().Specify(spec).AsNoTracking().ToListAsync(ct);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        //await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<int> UpdateRangeAsync(List<T> entities, CancellationToken ct = default)
    {
        if (entities == null || entities.Count == 0)
            return 0;

        _context.Set<T>().UpdateRange(entities);
        return await _context.SaveChangesAsync(ct);


    }

    
}
