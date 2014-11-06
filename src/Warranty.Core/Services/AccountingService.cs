namespace Warranty.Core.Services
{
    using System;
    using System.Linq.Expressions;
    using Accounting.Client;

    public class AccountingService : IAccountingService
    {
        private readonly IAccountingClient _accountingClient;

        public AccountingService(IAccountingClient accountingClient)
        {
            _accountingClient = accountingClient;
        }

        public TResponse Execute<TResponse>(Expression<Func<IAccountingClient, TResponse>> expression)
        {
            try
            {
                var func = expression.Compile();
                return func(_accountingClient);
            }
            catch (Exception ex)
            {
                return default(TResponse);
            }
        }
    }
}