namespace Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem
{
    using System.Collections.Generic;
    using System.Linq;
    using Services;

    public class ConstructionVendorQueryHandler : IQueryHandler<ConstructionVendorQuery, IEnumerable<ConstructionVendorModel>
        >
    {
        private readonly IAccountingService _accountingService;

        public ConstructionVendorQueryHandler(IAccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        public IEnumerable<ConstructionVendorModel> Handle(ConstructionVendorQuery message)
        {
            var vendorInfos = _accountingService.Execute(x => x.Get.VendorInfo(
                    new
                    {
                        JobNumber = message.JobNumber,
                        CostCode = message.CostCode
                    }));

            if (vendorInfos == null)
                return new List<ConstructionVendorModel>();
            
            IEnumerable<VendorInfoDetails> infoObjects = vendorInfos.Details.ToObject<List<VendorInfoDetails>>();

            return infoObjects.GroupBy(infoObj => infoObj.VendorNumber).Select(g =>
                new ConstructionVendorModel()
                {
                    VendorNumber = g.Key,
                    VendorName = g.First().VendorName,
                    VendorEmail = g.First().VendorEmail,
                    VendorPhoneNumbers = g.Select(x => string.Format("{0} ({1})", x.VendorPhone, x.PhoneType))
                });
        }


        public class VendorInfoDetails
        {
            public string VendorName { get; set; }
            public string VendorPhone { get; set; }
            public string PhoneType { get; set; }
            public string VendorEmail { get; set; }
            public string VendorNumber { get; set; }
            public string HeldStatus { get; set; }
        }
    }
}
