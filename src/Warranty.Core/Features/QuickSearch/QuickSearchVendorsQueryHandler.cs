namespace Warranty.Core.Features.QuickSearch
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using NPoco;
    using Security;
    using Services;

    public class QuickSearchVendorsQueryHandler : IQueryHandler<QuickSearchVendorsQuery, IEnumerable<QuickSearchCallVendorModel>>
    {
        private readonly IAccountingService _accountingService;

        public QuickSearchVendorsQueryHandler(IAccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        public IEnumerable<QuickSearchCallVendorModel> Handle(QuickSearchVendorsQuery query)
        {
            var vendors = _accountingService.Execute(x => x.Get.VendorSearchResult(
                new
                {
                    searchString = query.Query,
                }));

            if (vendors == null) return new List<QuickSearchCallVendorModel>();

            IEnumerable<QuickSearchCallVendorModel> vendorsResult = vendors.Details.ToObject<List<QuickSearchCallVendorModel>>();

            return vendorsResult;
        }
    }
}