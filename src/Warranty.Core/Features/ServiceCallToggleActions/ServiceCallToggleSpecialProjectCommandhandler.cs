namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;
    using InnerMessages;
    using NServiceBus;
    using NPoco;
    using ActivityLogger;
    using Entities;
    using Enumerations;

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
                serviceCall.SpecialProjectReason = (serviceCall.IsSpecialProject) ? message.Text : string.Empty;
                serviceCall.SpecialProjectDate = (serviceCall.IsSpecialProject) ? DateTime.UtcNow : (DateTime?)null;

                _database.Update(serviceCall);
                _bus.Send<NotifyServiceCallSpecialProjectStatusChanged>(x =>
                    {
                        x.ServiceCallId = serviceCall.ServiceCallId;
                        x.SpecialProjectDate = serviceCall.SpecialProjectDate;
                        x.SpecialProjectReason = serviceCall.SpecialProjectReason;
                    });

                var activityName = serviceCall.IsSpecialProject ? "Special Project" : "Not Special Project";

                _logger.Write(activityName, message.Text, message.ServiceCallId, ActivityType.SpecialProject, ReferenceType.ServiceCall);
            }
        }
    }
}
