﻿namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Security;

    public class AddPaymentOnHoldCommandHandler : ICommandHandler<AddPaymentOnHoldCommand, AddPaymentOnHoldCommandHandlerResponse>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public AddPaymentOnHoldCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus, IUserSession userSession)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
            _userSession = userSession;
        }

        public AddPaymentOnHoldCommandHandlerResponse Handle(AddPaymentOnHoldCommand message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);

                var newStatus = PaymentStatus.RequestedHold;
                payment.PaymentStatus = newStatus;
                payment.HoldComments = message.Message;
                payment.HoldDate = DateTime.UtcNow;

                _database.Update(payment);

                _activityLogger.Write("Payment hold requested", message.Message, payment.PaymentId, ActivityType.PaymentOnHold, ReferenceType.LineItem);

                _bus.Send<NotifyPaymentOnHold>(x => 
                { 
                    x.PaymentId = payment.PaymentId;
                    x.Username = _userSession.GetCurrentUser().LoginName;
                });

                return new AddPaymentOnHoldCommandHandlerResponse
                {
                    NewStatusDisplayName = newStatus.DisplayName,
                    Date = payment.HoldDate.Value
                };
            }
        }
    }
}