namespace Warranty.Core.Services
{
    using System;
    using System.Linq.Expressions;
    using Accounting.Client;
    using System.Configuration;
    using Common.Api.Http;
    using Accounting.API.Models;
    using Accounting.Client.API.Endpoints;

    public class AccountingService : IAccountingService
    {
        private readonly IAccountingClient _accountingClient;
        private readonly AccountingClientSystemMonitor _monitor;
        private readonly IApiClient _client;
        private readonly IApiConverter _converter;

        public AccountingService(IAccountingClient accountingClient, AccountingClientSystemMonitor monitor, IApiClient client, IApiConverter converter)
        {
            _accountingClient = accountingClient;
            _monitor = monitor;
            _client = client;
            _converter = converter;
        }

        private Uri AccountingBaseAddress
        {
            get
            {
                return new Uri(ConfigurationManager.AppSettings["Accounting.API.BaseUri"]);
            }
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

        public HomeownerIsPayableValidationResponseDto GetHomeownerIdIfValid(string name, string address)
        {
            var proxy = new HomeownerEndpointProxy(AccountingBaseAddress.AbsoluteUri, _client, _converter);
            var result = proxy.ExistsInSupplyMaster(name, address);

            return result;
        }
    }
}