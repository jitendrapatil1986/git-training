using System;

namespace Warranty.Core.ApprovalInfrastructure.Interfaces
{
    public interface IApprovalService<TEntity>
    {
        TEntity Approve(Guid id);
        TEntity Deny(Guid id);
    }
}