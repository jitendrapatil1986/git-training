namespace Warranty.Core.Features.AddServiceCallNote
{
    using System;
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddServiceCallNoteCommandHandler: ICommandHandler<AddServiceCallNoteCommand, AddServiceCallNoteModel>
    {
        private readonly IDatabase _database;

        public AddServiceCallNoteCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public AddServiceCallNoteModel Handle(AddServiceCallNoteCommand message)
        {
            using (_database)
            {
                var newServiceCallNote = new ServiceCallNote
                    {
                        ServiceCallId = message.ServiceCallId,
                        Note = message.Note,
                    };

                ServiceCallLineItemStatus serviceLineStatus = null;

                if (message.ServiceCallLineItemId.GetValueOrDefault() != Guid.Empty)
                {
                    var updateServiceCallLine = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                    
                    if (updateServiceCallLine.ServiceCallLineItemStatus == ServiceCallLineItemStatus.Open)
                        updateServiceCallLine.ServiceCallLineItemStatus = ServiceCallLineItemStatus.InProgress;

                    serviceLineStatus = updateServiceCallLine.ServiceCallLineItemStatus;

                    _database.Update(updateServiceCallLine);

                    newServiceCallNote.ServiceCallLineItemId = message.ServiceCallLineItemId;
                }
                
                _database.Insert(newServiceCallNote);

                var model = new AddServiceCallNoteModel
                    {
                        ServiceCallNoteId = newServiceCallNote.ServiceCallNoteId,
                        ServiceCallId = newServiceCallNote.ServiceCallId,
                        ServiceCallLineItemId = newServiceCallNote.ServiceCallLineItemId,
                        Note = newServiceCallNote.Note,
                        CreatedBy = newServiceCallNote.CreatedBy,
                        CreatedDate = newServiceCallNote.CreatedDate,
                    };

                if (serviceLineStatus != null)
                    model.ServiceCallLineItemStatus = serviceLineStatus;

                return model;
            }
        }
    }
}