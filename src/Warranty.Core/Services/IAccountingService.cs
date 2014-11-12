namespace Warranty.Core.Services
{
    using System;
    using System.Linq.Expressions;
    using Accounting.Client;

    public interface IAccountingService
    {
        TResponse Execute<TResponse>(Expression<Func<IAccountingClient, TResponse>> func);
    }
}