namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Events.Payment;
    using Core.Entities;
    using Core.Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class RequestPaymentResponseHandler : IHandleMessages<RequestPaymentResponse>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public RequestPaymentResponseHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public void Handle(RequestPaymentResponse message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentIdentifier);
                payment.JdeIdentifier = message.JdeIdentifier;
                payment.PaymentStatus = PaymentStatus.Pending;
                _database.Update(payment);

                var backcharge = _database.SingleOrDefault<Backcharge>("WHERE PaymentId = @0", message.PaymentIdentifier);
                
                if (backcharge != null)
                {
                    _bus.SendLocal<NotifyBackchargeRequested>(x =>
                    {
                        x.BackchargeId = backcharge.BackchargeId;
                        x.Username = backcharge.Username;
                        x.EmployeeNumber = backcharge.EmployeeNumber;
                    });
                }
            }
        }
    }
}