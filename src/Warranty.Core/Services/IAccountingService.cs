namespace Warranty.Core.Services
{
    using System;
    using System.Linq.Expressions;
    using Accounting.Client;
    using Accounting.API.Models;

    public interface IAccountingService
    {
        TResponse Execute<TResponse>(Expression<Func<IAccountingClient, TResponse>> func);
        HomeownerIsPayableValidationResponseDto GetHomeownerIdIfValid(string name, string address);
    }
}