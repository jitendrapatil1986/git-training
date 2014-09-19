namespace Warranty.Core.Features.AddServiceCallNote
{
    using System;
    using Entities;
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
                        Note = message.Note
                    };

                if (message.ServiceCallLineItemId != Guid.Empty)
                {
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
                        CreatedDate = newServiceCallNote.CreatedDate
                    };

                return model;
            }
        }
    }
}