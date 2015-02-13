namespace Warranty.Core.Features.DeleteServiceCallNote
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class DeleteServiceCallNoteCommandHandler : ICommandHandler<DeleteServiceCallNoteCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;

        public DeleteServiceCallNoteCommandHandler(IDatabase database, IActivityLogger activityLogger)
        {
            _database = database;
            _activityLogger = activityLogger;
        }

        public void Handle(DeleteServiceCallNoteCommand message)
        {
            using (_database)
            {
                var serviceCallNote = _database.SingleById<ServiceCallNote>(message.ServiceCallNoteId);
                _database.Delete(serviceCallNote);

                _activityLogger.Write("Service Call note deleted.", "Note: " + message.Note, message.ServiceCallId, ActivityType.DeleteNote, ReferenceType.ServiceCall);
            }
        }
    }
}