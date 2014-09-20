using NPoco;
using Warranty.Core.ActivityLogger;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Features.ServiceCallToggleActions
{
    public class ServiceCallToggleEscalateCommandCommandhandler : ICommandHandler<ServiceCallToggleEscalateCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public ServiceCallToggleEscalateCommandCommandhandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(ServiceCallToggleEscalateCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                serviceCall.IsEscalated = !serviceCall.IsEscalated;
                _database.Update(serviceCall);

                var activityName = serviceCall.IsEscalated ? "Escalate" : "Deescalate";

                _logger.Write(activityName, message.Text, message.ServiceCallId, ActivityType.SpecialProject, ReferenceType.ServiceCall);
            }
        }
    }
}