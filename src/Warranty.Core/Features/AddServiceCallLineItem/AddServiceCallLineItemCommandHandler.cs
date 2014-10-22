namespace Warranty.Core.Features.AddServiceCallLineItem
{
    using System;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class AddServiceCallLineItemCommandHandler : ICommandHandler<AddServiceCallLineItemCommand, AddServiceCallLineItemModel>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public AddServiceCallLineItemCommandHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
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
                        ProblemDescription = message.ProblemDescription,
                        ServiceCallLineItemStatus = ServiceCallLineItemStatus.Open,
                    };

                _database.Insert(newServiceLineItem);

                _bus.Send<NotifyServiceCallLineItemCreated>(x =>
                    {
                        x.ServiceCallLineItemId = newServiceLineItem.ServiceCallLineItemId;
                    });

                var model = new AddServiceCallLineItemModel
                    {
                        ServiceCallLineItemId = newServiceLineItem.ServiceCallLineItemId,
                        ServiceCallId = newServiceLineItem.ServiceCallId,
                        LineNumber = newServiceLineItem.LineNumber,
                        ServiceCallLineItemStatus = newServiceLineItem.ServiceCallLineItemStatus,
                    };

                return model;
            }
        }
    }
}
