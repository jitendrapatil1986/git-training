using System;
using Warranty.Core.ApprovalInfrastructure.Interfaces;

namespace Warranty.Core.ApprovalInfrastructure
{
    public abstract class ApprovalService<TEntity> : IApprovalService<TEntity> 
    {
        public abstract TEntity Approve(Guid id);
        public abstract TEntity Deny(Guid id);
    }
}