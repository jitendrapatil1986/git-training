namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class JobNoteMap: AuditableEntityMap<JobNote>
    {
        public JobNoteMap()
        {
            TableName("JobNotes")
                .PrimaryKey("JobNoteId", false)
                .Columns(x =>
                {
                    x.Column(y => y.JobId);
                    x.Column(y => y.Note);
                });
        }
    }
}