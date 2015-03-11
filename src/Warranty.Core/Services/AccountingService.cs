namespace Warranty.Core.Services
{
    using System;
    using System.Linq.Expressions;
    using Accounting.Client;

    public class AccountingService : IAccountingService
    {
        private readonly IAccountingClient _accountingClient;
        private readonly AccountingClientSystemMonitor _monitor;

        public AccountingService(IAccountingClient accountingClient, AccountingClientSystemMonitor monitor)
        {
            _accountingClient = accountingClient;
            _monitor = monitor;
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
                _monitor.LogException(expression.Body.ToString(), ex);
                return default(TResponse);
            }
        }
    }
}