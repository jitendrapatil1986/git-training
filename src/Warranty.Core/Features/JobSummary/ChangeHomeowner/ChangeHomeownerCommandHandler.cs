namespace Warranty.Core.Features.JobSummary.ChangeHomeowner
{
    using Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class ChangeHomeownerCommandHandler : ICommandHandler<ChangeHomeownerCommand>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public ChangeHomeownerCommandHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public void Handle(ChangeHomeownerCommand message)
        {
            using (_database)
            {
                const string sql = @"SELECT TOP 1 ho.HomeownerNumber
                                    FROM Jobs j
                                    INNER JOIN HomeOwners ho
                                    ON j.JobId = ho.JobId
                                    WHERE j.JobId = @0
                                    ORDER BY ho.HomeownerNumber DESC";

                var currentHomeownerNumber = _database.SingleOrDefault<int>(sql, message.Model.JobId);

                var newHomeowner = new HomeOwner
                    {
                        JobId = message.Model.JobId,
                        HomeOwnerNumber = currentHomeownerNumber + 1,
                        HomeOwnerName = message.Model.NewHomeownerName,
                        HomePhone = message.Model.NewHomeownerHomePhone,
                        OtherPhone = message.Model.NewHomeownerOtherPhone,
                        WorkPhone1 = message.Model.NewHomeownerWorkPhone1,
                        WorkPhone2 = message.Model.NewHomeownerWorkPhone2,
                        EmailAddress = message.Model.NewHomeownerEmailAddress,
                    };

                _database.Insert(newHomeowner);

                if (message.Model.AdditionalPhoneContacts != null)
                {
                    foreach (var additionalPhoneContact in message.Model.AdditionalPhoneContacts)
                    {
                        var additionalPhoneContacts = new HomeownerContact
                            {
                                HomeownerId = newHomeowner.HomeOwnerId,
                                ContactType = additionalPhoneContact.ContactType,
                                ContactValue = additionalPhoneContact.ContactValue
                            };

                        _database.Insert(additionalPhoneContacts);
                    }
                }

                if (message.Model.AdditionalEmailContacts != null)
                {
                    foreach (var additionalEmailContact in message.Model.AdditionalEmailContacts)
                    {
                        var additionalEmailContacts = new HomeownerContact
                            {
                                HomeownerId = newHomeowner.HomeOwnerId,
                                ContactType = additionalEmailContact.ContactType,
                                ContactValue = additionalEmailContact.ContactValue
                            };

                        _database.Insert(additionalEmailContacts);
                    }
                }

                var job = _database.SingleById<Job>(message.Model.JobId);
                job.CurrentHomeOwnerId = newHomeowner.HomeOwnerId;
                _database.Update(job);

                _bus.Send<NotifyJobHomeownerChanged>(x => { x.JobId = job.JobId; });
            }
        }
    }
}