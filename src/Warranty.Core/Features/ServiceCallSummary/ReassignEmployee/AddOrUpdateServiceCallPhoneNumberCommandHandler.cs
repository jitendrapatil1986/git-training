namespace Warranty.Core.Features.ServiceCallSummary.ReassignEmployee
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddOrUpdateServiceCallPhoneNumberCommandHandler : ICommandHandler<AddOrUpdateServiceCallPhoneNumberCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public AddOrUpdateServiceCallPhoneNumberCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(AddOrUpdateServiceCallPhoneNumberCommand message)
        {
            using (_database)
            {
                const string queryHomeOwner = @"SELECT ho.* 
                                        FROM [ServiceCalls] wc
                                        INNER JOIN Jobs j
                                            ON wc.JobId = j.JobId
                                        INNER JOIN HomeOwners ho
                                            ON j.CurrentHomeOwnerId = ho.HomeOwnerId                                
                                        WHERE wc.ServiceCallId=@0";

                var homeOwner = _database.SingleOrDefault<HomeOwner>(queryHomeOwner, message.Pk);

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