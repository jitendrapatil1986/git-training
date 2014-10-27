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
            const string sql = @"SELECT ContactValue, ContactType
                                FROM HomeownerContacts
						        WHERE HomeownerId = @0
                                ORDER BY CreatedDate";

            var contacts = _database.Fetch<AdditionalContactsModel.AdditionalContact>(sql, homeownerId.ToString());

            var result = new AdditionalContactsModel
            {
                AdditionalEmailContacts = contacts.Where(x => x.ContactType == HomeownerContactType.Email),
                AdditionalPhoneContacts = contacts.Where(x => x.ContactType == HomeownerContactType.Phone)
            };

            return result;
        }
    }
}
