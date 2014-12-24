namespace Warranty.Core.Features.Homeowner
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddOrUpdateEmailCommandHandler : ICommandHandler<AddOrUpdateEmailCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public AddOrUpdateEmailCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(AddOrUpdateEmailCommand message)
        {
            using (_database)
            {
                var homeOwner = _database.SingleById<HomeOwner>(message.Pk);

                var oldEmail = homeOwner.EmailAddress;

                homeOwner.EmailAddress = message.Value;

                _database.Update(homeOwner);

                var logDetails = string.Format("Old Email: {0}, New Email {1}", oldEmail, message.Value);
                _logger.Write("Homeowner email change", logDetails, homeOwner.HomeOwnerId, ActivityType.ChangeEmail, ReferenceType.Homeowner);
            }
        }
    }
}