using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Features.ServiceCallToggleActions
{
    public class ServiceCallToogleSpecialProjectCommandhandler : ICommandHandler<ServiceCallToogleSpecialProjectCommand>
    {
        private readonly IDatabase _database;

        public ServiceCallToogleSpecialProjectCommandhandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(ServiceCallToogleSpecialProjectCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                serviceCall.IsSpecialProject = !serviceCall.IsSpecialProject;
                _database.Update(serviceCall);
                Logactivity(message, serviceCall);
            }
        }

        private void Logactivity(ServiceCallToogleSpecialProjectCommand message, ServiceCall serviceCall)
        {
            var activityName = serviceCall.IsSpecialProject ? "Mark as Special Project" : "Unmark as Special Project";
            var activityLog = new ActivityLog
                {
                    ActivityName = activityName,
                    ActivityType = ActivityType.SpecialProject,
                    Details = message.Text,
                    ReferenceId = serviceCall.ServiceCallId,
                    ReferenceType = ReferenceType.ServiceCall,
                };

            _database.Insert(activityLog);
        }
    }
}
