namespace Warranty.Server.Handlers.Jobs
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyJobHomeownerChangedHandler : IHandleMessages<NotifyJobHomeownerChanged>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyJobHomeownerChangedHandler(IBus bus, IDatabase database )
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
                        x.JobNumber = job.JobNumber;
                        x.HomeownerName = homeowner.HomeOwnerName;
                        x.HomeownerHomePhone = homeowner.HomePhone;
                        x.HomeownerEmailAddress = homeowner.EmailAddress;
                    });
            }
        }
    }
}