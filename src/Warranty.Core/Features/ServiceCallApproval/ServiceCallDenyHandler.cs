using System;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Core.Exceptions;
using Warranty.InnerMessages;

namespace Warranty.Core.Features.ServiceCallApproval
{
    public class ServiceCallDenyHandler : ICommandHandler<DeleteServiceCallCommand>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public ServiceCallDenyHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(DeleteServiceCallCommand message)
        {
            if(message.ServiceCallId == Guid.Empty)
                throw new ArgumentNullException("message.ServiceCallId");
            
            var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
            if(serviceCall == null)
                throw new DeleteServiceCallException("Service call cannot be deleted because it does not exist or has already been deleted.");

            if(ServiceCallHasPendingPaymentsOrPurchaseOrders(message.ServiceCallId))
                throw new DeleteServiceCallException("Service call cannot be deleted because it has pending payments and/or purchase orders.");
            
            _database.Delete<ServiceCallAttachment>(@"WHERE ServiceCallId = @0", message.ServiceCallId);
            _database.Delete<ServiceCallNote>(@"WHERE ServiceCallId = @0", message.ServiceCallId);
            _database.Delete<ServiceCallLineItem>(@"WHERE ServiceCallId = @0", message.ServiceCallId);

            _database.Delete(serviceCall);

            _bus.Send<NotifyServiceCallDeleted>(x =>
            {
                x.ServiceCallId = message.ServiceCallId;
            });
        }

        private bool ServiceCallHasPendingPaymentsOrPurchaseOrders(Guid serviceCallId)
        {
            var paymentCount = _database.SingleOrDefault<int>(
                                @"SELECT count(*)
                                FROM ServiceCalls SC
                                INNER JOIN ServiceCallLineItems SCLI on SCLI.ServiceCallId = SC.ServiceCallId
                                INNER JOIN Payments P on SCLI.ServiceCallLineItemId = P.ServiceCallLineItemId                         
                                WHERE SC.ServiceCallId = @0", serviceCallId);

            if (paymentCount > 0)
                return true;

            var purchaseOrderCount = _database.SingleOrDefault<int>(
                                @"SELECT COUNT(*) 
                                FROM PurchaseOrders po
                                JOIN ServiceCallLineItems li on li.ServiceCallLineItemId = po.ServiceCallLineItemId
                                JOIN ServiceCalls sc on li.ServiceCallId = sc.ServiceCallId
                                WHERE sc.ServiceCallId = @0", serviceCallId);

            return purchaseOrderCount > 0;
        }
    }
}
