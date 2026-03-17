using CO.Domain.Contracts.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Domain.Contracts.Interfaces.Common;

public interface IUnitOfWork
{
    IClientRepository ClientRepository { get; }
    IStaffMemberRepository StaffMemberRepository { get; }
    
    Task<int> CompleteAsync(CancellationToken ct = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    void ClearChangeTracker();
}
