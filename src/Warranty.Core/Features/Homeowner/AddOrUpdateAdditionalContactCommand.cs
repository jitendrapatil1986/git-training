namespace Warranty.Core.Features.Homeowner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddOrUpdateAdditionalContactCommand : ICommand
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

    public class AddOrUpdateAdditionalContactCommandHandler : ICommandHandler<AddOrUpdateAdditionalContactCommand>
    {
        private readonly IDatabase _database;

        public AddOrUpdateAdditionalContactCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(AddOrUpdateAdditionalContactCommand message)
        {
            using (_database)
            {
                if (message.IsNewContact())
                {
                    var newcontact = new HomeownerContact
                        {
                            ContactType = HomeownerContactType.FromValue(message.HomeownerContactTypeValue),
                            ContactValue = message.Value["contactValue"],
                            ContactLabel = message.Value["contactLabel"],
                            HomeownerId = message.HomeownerId
                        };
                    _database.Insert(newcontact);
                    return;
                }

                var contact = _database.SingleById<HomeownerContact>(message.HomeownerContactId);
                contact.ContactValue = message.Value["contactValue"];
                contact.ContactValue = message.Value["contactLabel"];
                _database.Update(contact);
            }
        }
    }
}