namespace Warranty.Core.Features.EditServiceCallLineItem
{
    using System;
    using Entities;
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
                updateServiceCallLine.ProblemCode = message.ProblemCode;
                updateServiceCallLine.ProblemDetailCode = message.ProblemDetailCode;
                updateServiceCallLine.ProblemJdeCode = message.ProblemJdeCode;
                updateServiceCallLine.ProblemDescription = message.ProblemDescription;
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
