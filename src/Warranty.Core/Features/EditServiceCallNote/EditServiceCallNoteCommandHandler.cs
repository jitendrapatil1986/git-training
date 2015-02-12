namespace Warranty.Core.Features.EditServiceCallNote
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class EditServiceCallNoteCommandHandler : ICommandHandler<EditServiceCallNoteCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;

        public EditServiceCallNoteCommandHandler(IDatabase database, IActivityLogger activityLogger)
        {
            _database = database;
            _activityLogger = activityLogger;
        }

        public void Handle(EditServiceCallNoteCommand message)
        {
            using (_database)
            {
                var serviceCallNote = _database.SingleById<ServiceCallNote>(message.ServiceCallNoteId);
                serviceCallNote.Note = message.Note;
                
                _database.Update(serviceCallNote);

                _activityLogger.Write("Service Call note updated.", "Note: " + message.Note, message.ServiceCallId, ActivityType.ChangeNote, ReferenceType.ServiceCall);
            }
        }
    }
}