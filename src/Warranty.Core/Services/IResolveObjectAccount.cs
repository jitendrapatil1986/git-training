namespace Warranty.Core.Services
{
    using Entities;

    public interface IResolveObjectAccount
    {
        string ResolveLaborObjectAccount(Job job, ServiceCall serviceCall);
        string ResolveMaterialObjectAccount(Job job, ServiceCall serviceCall);
    }
}
