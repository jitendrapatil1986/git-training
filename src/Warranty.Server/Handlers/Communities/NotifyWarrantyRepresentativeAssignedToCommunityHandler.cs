using NServiceBus;
using Warranty.Events;
using Warranty.InnerMessages;

namespace Warranty.Server.Handlers.Communities
{
    public class NotifyWarrantyRepresentativeAssignedToCommunityHandler : IHandleMessages<NotifyWarrantyRepresentativeAssignedToCommunity>
    {
        private readonly IBus _bus;

        public NotifyWarrantyRepresentativeAssignedToCommunityHandler(IBus bus)
        {
            _bus = bus;
        }

        public void Handle(NotifyWarrantyRepresentativeAssignedToCommunity message)
        {
            _bus.Publish(new WarrantyRepresentativeAssignedToCommunity
            {
                CommunityId = message.CommunityId,
                CommunityNumber = message.CommunityNumber,
                EmployeeId = message.EmployeeId,
                EmployeeNumber = message.EmployeeNumber
            });
        }
    }
}