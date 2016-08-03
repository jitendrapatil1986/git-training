using AutoMapper;
using Warranty.Core.Entities;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Core.Features.Homeowner
{
    public class CreateNewHomeOwnerCommandHandler : ICommandHandler<CreateNewHomeOwnerCommand, HomeOwner>
    {
        private readonly IHomeOwnerService _homeOwnerService;

        public CreateNewHomeOwnerCommandHandler(IHomeOwnerService homeOwnerService)
        {
            _homeOwnerService = homeOwnerService;
        }

        public HomeOwner Handle(CreateNewHomeOwnerCommand message)
        {
            var homeOwner = Mapper.Map<HomeOwner>(message);
            homeOwner.HomeOwnerNumber = 1;

            return _homeOwnerService.Create(homeOwner);
        }
    }
}