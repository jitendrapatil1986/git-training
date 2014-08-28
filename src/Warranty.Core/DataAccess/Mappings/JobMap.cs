namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class JobMap : AuditableEntityMap<Job>
    {
        public JobMap()
        {
            TableName("Jobs")
                 .PrimaryKey(x=>x.Id, false)
                 .Columns(x =>
                 {
                     x.Column(y => y.Id).WithName("JobId");
                     x.Column(y => y.Builder);
                     x.Column(y => y.CloseDate);
                     x.Column(y => y.Community);
                     x.Column(y => y.CurrentHomeowner);
                     x.Column(y => y.Elevation);
                     x.Column(y => y.JobNumber);
                     x.Column(y => y.LegalDescription);
                     x.Column(y => y.PhysicalAddress);
                     x.Column(y => y.Plan);
                     x.Column(y => y.SalesConsultant);
                     x.Column(y => y.Swing);
                     x.Column(y => y.TotalPrice);
                     x.Column(y => y.WarrantyExpirationDate);
                 });
        }
    }
}