using CO.Domain.Contracts.Interfaces.Common;
using CO.Domain.Contracts.Interfaces.Repositories;
using CO.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Persistence.Contracts.Implementations.Interfaces;

public class UnitOfWork(
    IClientRepository clientRepository,
    IStaffMemberRepository staffMemberRepository,
    DBContext context


) : IUnitOfWork
{

    public IClientRepository ClientRepository { get; private set; } = clientRepository;
    public IStaffMemberRepository StaffMemberRepository { get; private set; } = staffMemberRepository;

    private IDbContextTransaction? _transaction;
    private readonly DBContext _context = context;



    public async Task<int> CompleteAsync(CancellationToken ct = default)
    {
        var result = await _context.SaveChangesAsync(ct);
        return result!;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.Dispose();
            _transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void ClearChangeTracker()
    {
        _context.ChangeTracker.Clear();
    }


}
