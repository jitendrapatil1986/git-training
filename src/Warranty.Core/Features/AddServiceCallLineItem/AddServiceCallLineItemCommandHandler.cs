namespace Warranty.Core.Features.AddServiceCallLineItem
{
    using System;
    using Entities;
    using NPoco;

    public class AddServiceCallLineItemCommandHandler : ICommandHandler<AddServiceCallLineItemCommand, AddServiceCallLineItemModel>
    {
        private readonly IDatabase _database;

        public AddServiceCallLineItemCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public AddServiceCallLineItemModel Handle(AddServiceCallLineItemCommand message)
        {
            using (_database)
            {
                const string sql = @"SELECT TOP 1 LineNumber
                                     FROM ServiceCallLineItems
                                     WHERE ServiceCallId = @0
                                     ORDER BY LineNumber DESC";

                var newLine = _database.FirstOrDefault<int>(sql, message.ServiceCallId) + 1;

                var newServiceLineItem = new ServiceCallLineItem
                    {
                        ServiceCallId = message.ServiceCallId,
                        LineNumber = newLine,
                        ProblemCode = message.ProblemCode,
                        ProblemDescription = message.ProblemDescription
                    };

                _database.Insert(newServiceLineItem);

                var model = new AddServiceCallLineItemModel
                    {
                        ServiceCallLineItemId = newServiceLineItem.ServiceCallLineItemId,
                        ServiceCallId = newServiceLineItem.ServiceCallId,
                        LineNumber = newServiceLineItem.LineNumber
                    };

                return model;
            }
        }
    }
}
