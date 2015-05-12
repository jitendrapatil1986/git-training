using NServiceBus;
using Warranty.Events;
using Warranty.InnerMessages;

namespace Warranty.Server.Handlers.Communities
{
    public class NotifyCommunityWarrantyRepresentativeAssignmentChangedHandler : IHandleMessages<NotifyCommunityWarrantyRepresentativeAssignmentChanged>
    {
        private readonly IBus _bus;

        public NotifyCommunityWarrantyRepresentativeAssignmentChangedHandler(IBus bus)
        {
            _bus = bus;
        }

        public void Handle(NotifyCommunityWarrantyRepresentativeAssignmentChanged message)
        {
            _bus.Publish(new CommunityWarrantyRepresentativeAssignmentChanged
            {
                CommunityId = message.CommunityId,
                CommunityNumber = message.CommunityNumber,
                EmployeeId = message.EmployeeId,
                EmployeeNumber = message.EmployeeNumber
            });
        }
    }
}