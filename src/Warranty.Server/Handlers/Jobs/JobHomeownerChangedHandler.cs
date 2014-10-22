namespace Warranty.Server.Handlers.Jobs
{
    using System.Linq;
    using Core.Entities;
    using Core.Enumerations;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class JobHomeownerChangedHandler : IHandleMessages<NotifyJobHomeownerChanged>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public JobHomeownerChangedHandler(IBus bus, IDatabase database )
        {
            _bus = bus;
            _database = database;
        }
        public void Handle(NotifyJobHomeownerChanged message)
        {
            using (_database)
            {
                var job = _database.SingleById<Job>(message.JobId);
                var homeowner = _database.SingleById<HomeOwner>(job.CurrentHomeOwnerId);

                _bus.Publish<JobHomeownerChanged>(x =>
                    {
                        x.JobId = job.JobId;
                        x.HomeownerName = homeowner.HomeOwnerName;
                        x.HomeownerHomePhone = homeowner.HomePhone;
                        x.HomeownerEmailAddress = homeowner.EmailAddress;
                    });
            }
        }
    }
}