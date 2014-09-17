namespace Warranty.Core.Features.AddServiceCallNote
{
    using System;
    using Entities;
    using NPoco;

    public class AddServiceCallNoteCommandHandler: ICommandHandler<AddServiceCallNoteCommand, Guid>
    {
        private readonly IDatabase _database;

        public AddServiceCallNoteCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public Guid Handle(AddServiceCallNoteCommand message)
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

                return newServiceCallNote.ServiceCallNoteId;
            }
        }
    }
}