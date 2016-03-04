using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;
using Warranty.Core.ApprovalInfrastructure.Interfaces;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Exceptions;

namespace Warranty.Core.ApprovalInfrastructure.ApprovalServices
{
    using InnerMessages;
    using NServiceBus;

    public class ServiceCallApprovalService : IApprovalService<ServiceCall>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public ServiceCallApprovalService(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public ServiceCall Approve(Guid id)
        {
            return UpdateServiceCall(id, ServiceCallStatus.Open);
        }

        public void Deny(Guid id)
        {
            DeleteServiceCall(id);
        }

        private ServiceCall UpdateServiceCall(Guid id, ServiceCallStatus newStatus)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(id);
                serviceCall.ServiceCallStatus = newStatus;
                _database.Update(serviceCall);
                _bus.Send<NotifyServiceCallStatusChanged>(x =>
                {
                    x.ServiceCallId = serviceCall.ServiceCallId;
                });
                return serviceCall;
            }
        }

        private void CheckServiceCallCanBeDeleted(Guid id)
        {
            var errors = new List<string>();

            if (ServiceCallPayments(id) > 0)
                errors.Add("There is a pending payment.");

            if (ServiceCallPurchaseOrders(id) > 0)
                errors.Add("There is a purchase order.");

            if (errors.Any())
            {
                throw new DeleteServiceCallException(string.Join(" ", errors.ToArray()));
            }
        }

        private void DeleteServiceCall(Guid id)
        {
            CheckServiceCallCanBeDeleted(id);

            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(id);

                DeleteServiceCallAttachments(id);
                DeleteServiceCallNotes(id);
                DeleteServiceCallLineItems(id);

                _database.Delete(serviceCall);
                _bus.Send<NotifyServiceCallDeleted>(x =>
                {
                    x.ServiceCallId = serviceCall.ServiceCallId;
                });
            }
        }

        private int ServiceCallPayments(Guid id)
        {
            using (_database)
            {
                const string sql = @"SELECT count(*)
                                FROM ServiceCalls SC
                                INNER JOIN ServiceCallLineItems SCLI on SCLI.ServiceCallId = SC.ServiceCallId
                                INNER JOIN Payments P on SCLI.ServiceCallLineItemId = P.ServiceCallLineItemId                         
                                WHERE SC.ServiceCallId = @0";

                var result = _database.ExecuteScalar<int>(sql, id.ToString());

                return result;
            }
        }

        private void DeleteServiceCallNotes(Guid serviceCallId)
        {
            using (_database)
            {
                const string sql = @"WHERE ServiceCallId = @0";
                _database.Delete<ServiceCallNote>(sql, serviceCallId.ToString());
            }
        }

        private void DeleteServiceCallLineItems(Guid serviceCallId)
        {
            using (_database)
            {
                const string sql = @"WHERE ServiceCallId = @0";
                _database.Delete<ServiceCallLineItem>(sql, serviceCallId.ToString());
            }
        }

        private void DeleteServiceCallAttachments(Guid serviceCallId)
        {
            using (_database)
            {
                const string sql = @"WHERE ServiceCallId = @0";
                _database.Delete<ServiceCallAttachment>(sql, serviceCallId.ToString());
            }
        }

        private int ServiceCallPurchaseOrders(Guid serviceCallId)
        {
            using (_database)
            {
                const string sql = @"SELECT COUNT(*) 
                                    FROM PurchaseOrders po
                                    JOIN ServiceCallLineItems li on li.ServiceCallLineItemId = po.ServiceCallLineItemId
                                    JOIN ServiceCalls sc on li.ServiceCallId = sc.ServiceCallId
                                    WHERE sc.ServiceCallId = @0";
                return _database.ExecuteScalar<int>(sql, serviceCallId.ToString());
            } 
        }
    }
}
