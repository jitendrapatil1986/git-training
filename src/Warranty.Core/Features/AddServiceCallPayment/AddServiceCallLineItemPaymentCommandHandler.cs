namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddServiceCallLineItemPaymentCommandHandler : ICommandHandler<AddServiceCallLineItemPaymentCommand, Guid>
    {
        private readonly IDatabase _database;

        public AddServiceCallLineItemPaymentCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public Guid Handle(AddServiceCallLineItemPaymentCommand message)
        {
            using (_database)
            {
                var payment = new Payment
                {
                    Amount = message.Amount,
                    InvoiceNumber = message.InvoiceNumber,
                    ServiceCallLineItemId = message.ServiceCallLineItemId,
                    PaymentStatus = PaymentStatus.Pending,
                    VendorNumber = message.VendorNumber,
                    VendorName = message.VendorName,
                };

                _database.Insert(payment);


                if (message.IsBackcharge)
                {
                    var backcharge = new Backcharge
                    {
                        PaymentId = payment.PaymentId,

                        BackchargeVendorNumber = message.BackchargeVendorNumber,
                        BackchargeVendorName = message.BackchargeVendorName,
                        //BackchargeAmount = message.payment
                        BackchargeReason = message.BackchargeReason,
                        PersonNotified = message.PersonNotified,
                        PersonNotifiedPhoneNumber = message.PersonNotifiedPhoneNumber,
                        PersonNotifiedDate = message.PersonNotifiedDate,
                        BackchargeResponseFromVendor = message.BackchargeResponseFromVendor,
                    };
                    _database.Insert(backcharge);
                }

                return payment.PaymentId;
            }
        }
    }
}