namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class JobNoteMapping: AuditableEntityMapping<JobNote>
    {
        public JobNoteMapping()
        {
            Table("JobNotes");

            Id(x => x.JobNoteId, map => map.Generator(Generators.GuidComb));
            Property(x => x.JobId);
            Property(x => x.Note);
        }
    }
}