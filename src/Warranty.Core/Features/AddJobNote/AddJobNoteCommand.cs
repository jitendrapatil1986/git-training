namespace Warranty.Core.Features.AddJobNote
{
    using System;

    public class AddJobNoteCommand: ICommand<AddJobNoteModel>
    {
        public Guid JobId { get; set; }
        public string Note { get; set; }
    }
}