using System.Collections.Generic;
using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class GetJobsFromWarrantyRequestHandler : IRequestHandler<GetJobsFromWarrantyRequest, IEnumerable<string>>
    {
        private readonly IWarrantyDatabase _warrantyDatabase;

        public GetJobsFromWarrantyRequestHandler(IWarrantyDatabase warrantyDatabase)
        {
            _warrantyDatabase = warrantyDatabase;
        }

        public IEnumerable<string> Handle(GetJobsFromWarrantyRequest message)
        {
            return _warrantyDatabase.Fetch<string>("SELECT JobNumber FROM Jobs");
        }
    }

    public class GetJobsFromWarrantyRequest : IRequest<IEnumerable<string>>
    {

    }
}