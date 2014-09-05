namespace Warranty.Core.Features.AddServiceCallLineItem
{
    using System;
    using Entities;
    using NPoco;

    public class AddServiceCallLineItemCommandHandler : ICommandHandler<AddServiceCallLineItemCommand, bool>
    {
        private readonly IDatabase _database;

        public AddServiceCallLineItemCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public bool Handle(AddServiceCallLineItemCommand message)
        {
            using (_database)
            {
                const string sql = @"SELECT TOP 1 LineNumber
                                     FROM ServiceCallLineItems
                                     WHERE ServiceCallId = @0
                                     ORDER BY LineNumber DESC";

                var newLine = _database.FirstOrDefault<int>(sql, message.Model.ServiceCallId) + 1;

                var serviceLineItem = new ServiceCallLineItem
                    {
                        ServiceCallLineItemId = Guid.NewGuid(),
                        ServiceCallId = message.Model.ServiceCallId,
                        LineNumber = newLine,
                        ProblemCode = message.Model.ProblemCode,
                        ProblemDescription = message.Model.ProblemDescription
                    };

                _database.Insert(serviceLineItem);

                return true;
            }
        }
    }
}
