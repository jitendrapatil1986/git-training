namespace Warranty.Core.Features.EditServiceCallLineItem
{
    using System;
    using Entities;
    using NPoco;

    public class EditServiceCallLineCommandHandler : ICommandHandler<EditServiceCallLineCommand, Guid>
    {
        private readonly IDatabase _database;

        public EditServiceCallLineCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public Guid Handle(EditServiceCallLineCommand message)
        {
            using (_database)
            {
                var updateServiceCallLine = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                updateServiceCallLine.ProblemCode = message.ProblemCode;
                updateServiceCallLine.ProblemDescription = message.ProblemDescription;
                _database.Update(updateServiceCallLine);

                return updateServiceCallLine.ServiceCallLineItemId;
            }
        }
    }
}
