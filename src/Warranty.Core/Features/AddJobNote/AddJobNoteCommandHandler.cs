namespace Warranty.Core.Features.AddJobNote
{
    using Entities;
    using NPoco;

    public class AddJobNoteCommandHandler: ICommandHandler<AddJobNoteCommand, AddJobNoteModel>
    {
        private readonly IDatabase _database;

        public AddJobNoteCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public AddJobNoteModel Handle(AddJobNoteCommand message)
        {
            using (_database)
            {
                var newJobNote = new JobNote
                    {
                        JobId = message.JobId,
                        Note = message.Note,
                    };

                _database.Insert(newJobNote);

                var model = new AddJobNoteModel
                    {
                        JobNoteId = newJobNote.JobNoteId,
                        JobId = newJobNote.JobId,
                        Note = newJobNote.Note,
                        CreatedBy = newJobNote.CreatedBy,
                        CreatedDate = newJobNote.CreatedDate,
                    };

                return model;
            }
        }
    }
}