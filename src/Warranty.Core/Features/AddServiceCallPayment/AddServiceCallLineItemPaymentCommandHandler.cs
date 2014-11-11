﻿namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using Configurations;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class AddServiceCallLineItemPaymentCommandHandler : ICommandHandler<AddServiceCallLineItemPaymentCommand, Guid>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public AddServiceCallLineItemPaymentCommandHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public Guid Handle(AddServiceCallLineItemPaymentCommand message)
        {
            using (_database)
            {
                var job = _database.Single<Job>(@"SELECT j.*
                                                            FROM ServiceCallLineItems scli
                                                            INNER JOIN ServiceCalls sc
                                                                ON sc.ServiceCallId = scli.ServiceCallId
                                                            INNER JOIN Jobs j
                                                                ON J.JobId = sc.JobId
                                                            WHERE scli.ServiceCallLineItemId = @0", message.ServiceCallLineItemId);
                var payment = new Payment
                {
                    Amount = message.Amount,
                    InvoiceNumber = message.InvoiceNumber,
                    ServiceCallLineItemId = message.ServiceCallLineItemId,
                    PaymentStatus = PaymentStatus.Requested,
                    VendorNumber = message.VendorNumber,
                    VendorName = message.VendorName,
                    JobNumber = job.JobNumber,
                    CommunityNumber = string.IsNullOrEmpty(job.JobNumber) ? "" : job.JobNumber.Substring(0, 4),
                    CostCode = WarrantyCostCode.FromValue(message.SelectedCostCode).CostCode,
                    ObjectAccount = job.IsOutOfWarranty ? WarrantyConstants.OutOfWarrantyLaborCode : WarrantyConstants.InWarrantyLaborCode,
                };

                _database.Insert(payment);

                _bus.Send<NotifyRequestedPayment>(x =>
                {
                    x.PaymentId = payment.PaymentId;
                });

                if (message.IsBackcharge)
                {
                    var backcharge = new Backcharge
                    {
                        PaymentId = payment.PaymentId,

                        BackchargeVendorNumber = message.BackchargeVendorNumber,
                        BackchargeVendorName = message.BackchargeVendorName,
                        BackchargeAmount = message.BackchargeAmount,
                        BackchargeReason = message.BackchargeReason,
                        PersonNotified = message.PersonNotified,
                        PersonNotifiedPhoneNumber = message.PersonNotifiedPhoneNumber,
                        PersonNotifiedDate = message.PersonNotifiedDate,
                        BackchargeResponseFromVendor = message.BackchargeResponseFromVendor,
                        CostCode = WarrantyCostCode.FromValue(message.SelectedCostCode).CostCode,
                        BackchargeStatus = BackchargeStatus.Requested
                    };
                    _database.Insert(backcharge);

                    _bus.Send<NotifyRequestedBackcharge>(x =>
                    {
                        x.BackchargeId = backcharge.BackchargeId;
                    });
                }

                return payment.PaymentId;
            }
        }
    }
}