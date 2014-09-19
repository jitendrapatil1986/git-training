using NPoco;
using Warranty.Core.ActivityLogger;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Features.ServiceCallToggleActions
{
    public class ServiceCallToggleSpecialProjectCommandhandler : ICommandHandler<ServiceCallToggleSpecialProjectCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public ServiceCallToggleSpecialProjectCommandhandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(ServiceCallToggleSpecialProjectCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                serviceCall.IsSpecialProject = !serviceCall.IsSpecialProject;
                _database.Update(serviceCall);

                var activityName = serviceCall.IsSpecialProject ? "Special Project" : "Not Special Project";

                _logger.Write(activityName, message.Text, message.ServiceCallId, ActivityType.SpecialProject, ReferenceType.ServiceCall);
            }
        }
    }
}
