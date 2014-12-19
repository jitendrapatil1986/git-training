namespace Warranty.Core.Features.EditServiceCallLineItem
{
    using System;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class EditServiceCallLineCommandHandler : ICommandHandler<EditServiceCallLineCommand, Guid>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public EditServiceCallLineCommandHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public Guid Handle(EditServiceCallLineCommand message)
        {
            using (_database)
            {
                var updateServiceCallLine = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                updateServiceCallLine.ProblemCode = message.ProblemCode ?? updateServiceCallLine.ProblemCode;
                updateServiceCallLine.ProblemJdeCode = message.ProblemJdeCode ?? updateServiceCallLine.ProblemJdeCode;
                updateServiceCallLine.ProblemDescription = message.ProblemDescription ?? updateServiceCallLine.ProblemDescription;
                updateServiceCallLine.RootCause = message.RootCause == null ? updateServiceCallLine.RootCause : RootCause.FromValue(Convert.ToInt16(message.RootCause)).DisplayName;
                updateServiceCallLine.RootProblem = message.RootProblem == null ? updateServiceCallLine.RootProblem : RootProblem.FromValue(Convert.ToInt16(message.RootProblem)).DisplayName;
                _database.Update(updateServiceCallLine);

                _bus.Send<NotifyServiceCallLineItemProblemChanged>(x =>
                    {
                        x.ServiceCallLineItemId = updateServiceCallLine.ServiceCallLineItemId;
                    });

                return updateServiceCallLine.ServiceCallLineItemId;
            }
        }
    }
}
