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
                var homeownerContacts = _database.Fetch<HomeownerContact>().Where(x => x.HomeownerId == homeowner.HomeOwnerId);

                _bus.Publish<JobHomeownerChanged>(x =>
                    {
                        x.JobId = job.JobId;
                        x.HomeownerName = homeowner.HomeOwnerName;
                        x.HomeownerHomePhone = homeowner.HomePhone;
                        x.HomeownerEmailAddress = homeowner.EmailAddress;
                        x.AdditionalPhoneContacts = homeownerContacts.Select(y => new JobHomeownerChanged.AdditionalPhoneContact
                            {
                                ContactType = HomeownerContactType.Phone.DisplayName,
                                ContactValue = y.ContactValue,
                            }).ToList();
                        x.AdditionalEmailContacts = homeownerContacts.Select(y => new JobHomeownerChanged.AdditionalEmailContact
                            {
                                ContactType = HomeownerContactType.Email.DisplayName,
                                ContactValue = y.ContactValue,
                            }).ToList();
                    });
            }
        }
    }
}