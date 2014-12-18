using System;

namespace Warranty.Core.Features.CreateServiceCall
{
    using Entities;
    using Enumerations;
    using NPoco;
    using Security;
    using Services;

    public class CreateServiceCallCommandHandler: ICommandHandler<CreateServiceCallCommand, Guid>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly IServiceCallCreateService _serviceCallCreateService;

        public CreateServiceCallCommandHandler(IDatabase database, IUserSession userSession, IServiceCallCreateService serviceCallCreateService)
        {
            _database = database;
            _userSession = userSession;
            _serviceCallCreateService = serviceCallCreateService;
        }

        public Guid Handle(CreateServiceCallCommand message)
        {
            var user = _userSession.GetCurrentUser();

            var status = ServiceCallStatus.Requested;
            if (user.IsInRole(UserRoles.WarrantyServiceCoordinator) || user.IsInRole(UserRoles.WarrantyServiceManager))
            {
                status = ServiceCallStatus.Open;
            }

            using (_database)
            {
                var serviceCallId = _serviceCallCreateService.Create(message.JobId, RequestType.WarrantyRequest, status);
                return serviceCallId;
            }
        }
    }
}
