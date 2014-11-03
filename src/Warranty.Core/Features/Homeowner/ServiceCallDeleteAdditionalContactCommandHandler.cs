namespace Warranty.Core.Features.Homeowner
{
    using Entities;
    using NPoco;

    public class ServiceCallDeleteAdditionalContactCommandHandler : ICommandHandler<ServiceCallDeleteAdditionalContactCommand>
    {
        private readonly IDatabase _database;

        public ServiceCallDeleteAdditionalContactCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(ServiceCallDeleteAdditionalContactCommand message)
        {
            using (_database)
            {
                var contact = _database.SingleById<HomeownerContact>(message.Id);
                if(contact != null)
                {
                    _database.Delete(contact);
                }
            }
        }
    }
}