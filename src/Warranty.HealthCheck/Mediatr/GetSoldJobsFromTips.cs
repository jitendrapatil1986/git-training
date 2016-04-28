using System;
using System.Collections.Generic;
using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class GetSoldJobsFromTipsRequestHandler : IRequestHandler<GetSoldJobsFromTipsRequest, IEnumerable<string>>
    {
        private readonly ITipsDatabase _tipsDatabase;

        public GetSoldJobsFromTipsRequestHandler(ITipsDatabase tipsDatabase)
        {
            _tipsDatabase = tipsDatabase;
        }

        public IEnumerable<string> Handle(GetSoldJobsFromTipsRequest message)
        {
            return _tipsDatabase.Fetch<string>(@"   SELECT aj.JobNumber
                                                    FROM		SALE_ApprovalJob aj WITH(NOLOCK)
                                                    INNER JOIN	SALE_Opportunity o WITH(NOLOCK) on o.SaleId = aj.SaleId
                                                    WHERE		aj.Active = 1 AND aj.CancelFlag = 0 AND o.SaleDate IS NOT NULL AND o.SaleDate > @0", 
                                            message.MaxSaleDate);
        }
    }

    public class GetSoldJobsFromTipsRequest : IRequest<IEnumerable<string>>
    {
        public GetSoldJobsFromTipsRequest() { }

        public GetSoldJobsFromTipsRequest(DateTime maxSaleDate)
        {
            MaxSaleDate = maxSaleDate;
        }

        public DateTime MaxSaleDate { get; set; }
    }
}