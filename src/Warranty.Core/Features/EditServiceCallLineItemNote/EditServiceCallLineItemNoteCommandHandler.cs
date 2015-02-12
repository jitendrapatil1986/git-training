namespace Warranty.Core.Features.EditServiceCallLineItemNote
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class EditServiceCallLineItemNoteCommandHandler : ICommandHandler<EditServiceCallLineItemNoteCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;

        public EditServiceCallLineItemNoteCommandHandler(IDatabase database, IActivityLogger activityLogger)
        {
            _database = database;
            _activityLogger = activityLogger;
        }

        public void Handle(EditServiceCallLineItemNoteCommand message)
        {
            using (_database)
            {
                var serviceCallNote = _database.SingleById<ServiceCallNote>(message.ServiceCallNoteId);
                serviceCallNote.Note = message.Note;

                _database.Update(serviceCallNote);

                _activityLogger.Write("Service Call Line Item note updated.", "Note: " + message.Note, message.ServiceCallLineItemId, ActivityType.ChangeNote, ReferenceType.ServiceCallLineItem);
            }
        }
    }
}