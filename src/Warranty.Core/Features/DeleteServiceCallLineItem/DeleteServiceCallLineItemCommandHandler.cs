namespace Warranty.Core.Features.DeleteServiceCallLineItem 
{
using System;
using ActivityLogger;
using Entities;
using Enumerations;
using InnerMessages;
using NPoco;
using NServiceBus;

public class DeleteServiceCallLineItemCommandHandler : ICommandHandler<DeleteServiceCallLineItemCommand>
{
    private readonly IDatabase _database;
    private readonly IActivityLogger _activityLogger;
    private readonly IBus _bus;
    public DeleteServiceCallLineItemCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus)
    {
        _database = database;
        _activityLogger = activityLogger;
        _bus = bus;
    }
    public void Handle(DeleteServiceCallLineItemCommand message)
    {
        using (_database.Transaction)
        {
            _database.BeginTransaction();
            _database.DeleteWhere<ServiceCallLineItem>("ServiceCallLineItemId = @0", message.ServiceCallLineItemId);

            _database.CompleteTransaction();

            _activityLogger.Write("Service Call Line Item deleted", string.Empty, message.ServiceCallLineItemId, ActivityType.LineItemDelete, ReferenceType.LineItem);
            _bus.Send<NotifyLineItemDeleted>(x =>
            {
                x.ServiceCallLineItemId = message.ServiceCallLineItemId;
            });
        }
    }
}
}