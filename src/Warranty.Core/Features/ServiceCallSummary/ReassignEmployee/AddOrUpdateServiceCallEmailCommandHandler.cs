namespace Warranty.Core.Features.ServiceCallSummary.ReassignEmployee
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddOrUpdateServiceCallEmailCommandHandler : ICommandHandler<AddOrUpdateServiceCallEmailCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public AddOrUpdateServiceCallEmailCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(AddOrUpdateServiceCallEmailCommand message)
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

                var oldEmail = homeOwner.EmailAddress;

                homeOwner.EmailAddress = message.Value;

                _database.Update(homeOwner);

                var logDetails = string.Format("Old Email: {0}, New Email {1}", oldEmail, message.Value);
                _logger.Write("Homeowner email change", logDetails, homeOwner.HomeOwnerId, ActivityType.ChangeEmail, ReferenceType.Homeowner);
            }
        }
    }
}