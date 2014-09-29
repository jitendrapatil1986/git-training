namespace Warranty.Core.Features.Homeowner
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddOrUpdatePhoneNumberCommandHandler : ICommandHandler<AddOrUpdatePhoneNumberCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public AddOrUpdatePhoneNumberCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(AddOrUpdatePhoneNumberCommand message)
        {
            using (_database)
            {
                var homeOwner = _database.SingleById<HomeOwner>(message.Pk);

                var phoneNumberType = PhoneNumberType.FromValue(message.PhoneNumberTypeValue);

                string oldPhone = string.Empty;

                if (phoneNumberType == PhoneNumberType.Home)
                {
                    oldPhone = homeOwner.HomePhone;
                    homeOwner.HomePhone = message.Value;

                }
                else if (phoneNumberType == PhoneNumberType.Mobile)
                {
                    oldPhone = homeOwner.OtherPhone;
                    homeOwner.OtherPhone = message.Value;
                }

                _database.Update(homeOwner);

                var logDetails = string.Format("Old Phone: {0}, New Phone {1}, Phone Number Type: {2}", oldPhone, message.Value, phoneNumberType.DisplayName);
                _logger.Write("Homeowner phone change", logDetails, homeOwner.HomeOwnerId, ActivityType.ChangePhone, ReferenceType.Homeowner);
            }
        }
    }
}