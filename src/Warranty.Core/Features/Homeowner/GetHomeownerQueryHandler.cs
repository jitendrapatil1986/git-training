using Warranty.Core.Entities;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Core.Features.Homeowner
{
    public class GetHomeOwnerQueryHandler : IQueryHandler<GetHomeOwnerQuery, HomeOwner>
    {
        private readonly IHomeOwnerService _homeOwnerService;

        public GetHomeOwnerQueryHandler(IHomeOwnerService homeOwnerService)
        {
            _homeOwnerService = homeOwnerService;
        }

        public HomeOwner Handle(GetHomeOwnerQuery query)
        {
            return _homeOwnerService.GetHomeOwnerByJobNumber(query.JobNumber);
        }
    }
}