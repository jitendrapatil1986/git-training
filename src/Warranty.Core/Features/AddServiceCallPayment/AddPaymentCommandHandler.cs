namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using Configurations;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Security;

    public class AddPaymentCommandHandler : ICommandHandler<AddPaymentCommand, Guid>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public AddPaymentCommandHandler(IDatabase database, IBus bus, IUserSession userSession)
        {
            _database = database;
            _bus = bus;
            _userSession = userSession;
        }

        public Guid Handle(AddPaymentCommand message)
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

                _bus.Send<NotifyPaymentRequested>(x =>
                {
                    x.PaymentId = payment.PaymentId;
                    x.Username = _userSession.GetCurrentUser().LoginName;
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

                    _bus.Send<NotifyBackchargeRequested>(x =>
                    {
                        x.BackchargeId = backcharge.BackchargeId;
                        x.Username = _userSession.GetCurrentUser().LoginName;
                    });
                }

                return payment.PaymentId;
            }
        }
    }
}