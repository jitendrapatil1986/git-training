namespace Warranty.Core.Features.DeleteServiceCallLineNote
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class DeleteServiceCallLineNoteCommandHandler : ICommandHandler<DeleteServiceCallLineNoteCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;

        public DeleteServiceCallLineNoteCommandHandler(IDatabase database, IActivityLogger activityLogger)
        {
            _database = database;
            _activityLogger = activityLogger;
        }

        public void Handle(DeleteServiceCallLineNoteCommand message)
        {
            using (_database)
            {
                var serviceCallNote = _database.SingleById<ServiceCallNote>(message.ServiceCallNoteId);
                _database.Delete(serviceCallNote);

                _activityLogger.Write("Service Call Line Item note deleted.", "Note: " + message.Note, message.ServiceCallLineItemId, ActivityType.DeleteNote, ReferenceType.ServiceCallLineItem);
            }
        }
    }
}