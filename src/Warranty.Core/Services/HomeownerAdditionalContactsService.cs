namespace Warranty.Core.Services
{
    using System;
    using System.Linq;
    using Enumerations;
    using Features;
    using NPoco;

    public class HomeownerAdditionalContactsService : IHomeownerAdditionalContactsService
    {
        private readonly IDatabase _database;

        public HomeownerAdditionalContactsService(IDatabase database )
        {
            _database = database;
        }

        public AdditionalContactsModel Get(Guid homeownerId)
        {
            using (_database)
            {
                const string sql = @"SELECT ContactValue, ContactType, ContactLabel, HomeownerId, HomeownerContactId, ContactType as HomeownerContactTypeValue
                                FROM HomeownerContacts
						        WHERE HomeownerId = @0
                                ORDER BY CreatedDate";

                var contacts = _database.Fetch<AdditionalContactsModel.AdditionalContact>(sql, homeownerId.ToString());

                var result = new AdditionalContactsModel
                    {
                        AdditionalEmailContacts = contacts.Where(x => x.HomeownerContactTypeValue == HomeownerContactType.Email.Value),
                        AdditionalPhoneContacts = contacts.Where(x => x.HomeownerContactTypeValue == HomeownerContactType.Phone.Value)
                    };

                return result;
            }
        }
    }
}
