using System;
using System.Collections.Generic;
using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class GetJobsWithHomeOwnerButNullCurrentHomeOwnerIdRequest : IRequest<IEnumerable<string>>
    {
        public DateTime RunDate { get; set; }

        public GetJobsWithHomeOwnerButNullCurrentHomeOwnerIdRequest()
        {
        }

        public GetJobsWithHomeOwnerButNullCurrentHomeOwnerIdRequest(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class GetJobsWithHomeOwnerButNullCurrentHomeOwnerIdHandler : IRequestHandler<GetJobsWithHomeOwnerButNullCurrentHomeOwnerIdRequest, IEnumerable<string>>
    {
        private readonly IWarrantyDatabase _warrantyDatabase;

        public GetJobsWithHomeOwnerButNullCurrentHomeOwnerIdHandler(IWarrantyDatabase warrantyDatabase)
        {
            _warrantyDatabase = warrantyDatabase;
        }

        public IEnumerable<string> Handle(GetJobsWithHomeOwnerButNullCurrentHomeOwnerIdRequest message)
        {
            var results = _warrantyDatabase.Fetch<string>(@"SELECT 
	                                                            J.JobNumber
                                                            FROM dbo.Jobs J
                                                            INNER JOIN dbo.Homeowners H
	                                                            ON H.JobId = J.JobId
                                                            WHERE J.CurrentHomeownerId IS NULL
                                                                AND J.CreatedDate > @0
                                                                AND J.CloseDate IS NOT NULL
                                                            ORDER BY J.CreatedDate;", message.RunDate);

            return results;
        }
    }
}