namespace Warranty.Core.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Enumerations;
    using NPoco;

    public class AdditionalContactsHandler : ICommandHandler<AdditionalContactsModel>
    {
        private readonly IDatabase _database;

        public AdditionalContactsHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(AdditionalContactsModel message)
        {
            using (_database)
            {
                _database.Execute("DELETE FROM HomeownerContacts where HomeownerId = @0", message.HomeownerId);
                PersistContacts(message.HomeownerId, message.AdditionalEmailContacts, HomeownerContactType.Email);
                PersistContacts(message.HomeownerId, message.AdditionalPhoneContacts, HomeownerContactType.Phone);
            }
        }

        private void PersistContacts(Guid homeownerId, IEnumerable<AdditionalContactsModel.AdditionalContact> list, HomeownerContactType type)
        {
            var contacts = list.Select(x => new HomeownerContact
                {
                    ContactType = type,
                    ContactValue = x.ContactValue,
                    HomeownerId = homeownerId
                });

            foreach (var homeownerContact in contacts)
            {
                _database.Insert(homeownerContact);
            }
        }
    }
}