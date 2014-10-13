using NPoco;
using Warranty.Core.ActivityLogger;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;
    using InnerMessages;
    using NServiceBus;

    public class ServiceCallToggleSpecialProjectCommandhandler : ICommandHandler<ServiceCallToggleSpecialProjectCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;
        private readonly IBus _bus;

        public ServiceCallToggleSpecialProjectCommandhandler(IDatabase database, IActivityLogger logger, IBus bus)
        {
            _database = database;
            _logger = logger;
            _bus = bus;
        }

        public void Handle(ServiceCallToggleSpecialProjectCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                serviceCall.IsSpecialProject = !serviceCall.IsSpecialProject;
                _database.Update(serviceCall);
                _bus.Send<NotifyServiceCallSpecialProjectUpdated>(x =>
                    {
                        x.ServiceCallId = serviceCall.ServiceCallId;
                        x.SpecialProjectDate = DateTime.UtcNow;
                        x.SpecialProjectReason = message.Text;
                    });

                var activityName = serviceCall.IsSpecialProject ? "Special Project" : "Not Special Project";

                _logger.Write(activityName, message.Text, message.ServiceCallId, ActivityType.SpecialProject, ReferenceType.ServiceCall);
            }
        }
    }
}
