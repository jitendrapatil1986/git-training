namespace Warranty.Core.Features.Homeowner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Enumerations;
    using Extensions;
    using NPoco;

    public class AddOrUpdateAdditionalContactCommand : ICommand<Guid>
    {
        public int HomeownerContactTypeValue { get; set; }
        public Guid HomeownerContactId { get; set; }
        public Guid HomeownerId { get; set; }
        public Dictionary<string, string> Value { get; set; }

        public bool IsNewContact()
        {
            return HomeownerContactId == Guid.Empty;
        }
    }

    public class AddOrUpdateAdditionalContactCommandHandler : ICommandHandler<AddOrUpdateAdditionalContactCommand, Guid>
    {
        private readonly IDatabase _database;

        public AddOrUpdateAdditionalContactCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public Guid Handle(AddOrUpdateAdditionalContactCommand message)
        {
            using (_database)
            {
                if (message.IsNewContact())
                {
                    var newcontact = new HomeownerContact
                        {
                            ContactType = HomeownerContactType.FromValue(message.HomeownerContactTypeValue),
                            ContactValue = message.HomeownerContactTypeValue == HomeownerContactType.Phone.Value ? message.Value["contactValue"].CleanPhoneNumber() : message.Value["contactValue"],
                            ContactLabel = message.Value["contactLabel"].Trim(),
                            HomeownerId = message.HomeownerId
                        };
                    _database.Insert(newcontact);
                    return newcontact.HomeownerContactId;
                }

                var contact = _database.SingleById<HomeownerContact>(message.HomeownerContactId);
                contact.ContactValue = message.HomeownerContactTypeValue == HomeownerContactType.Phone.Value
                                           ? message.Value["contactValue"].CleanPhoneNumber()
                                           : message.Value["contactValue"];
                contact.ContactLabel = message.Value["contactLabel"].Trim();
                _database.Update(contact);
                return contact.HomeownerContactId;
            }
        }
    }
}