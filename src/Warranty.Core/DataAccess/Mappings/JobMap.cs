using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class JobMap : AuditableEntityMap<Job>
    {
        public JobMap()
        {
            TableName("Jobs")
                .PrimaryKey("JobId", false)
                .Columns(x =>
                {
                    x.Column(y => y.JobNumber);
                    x.Column(y => y.CloseDate);
                    x.Column(y => y.AddressLine);
                    x.Column(y => y.City);
                    x.Column(y => y.StateCode);
                    x.Column(y => y.PostalCode);
                    x.Column(y => y.LegalDescription);
                    x.Column(y => y.CommunityId);
                    x.Column(y => y.CurrentHomeOwnerId);
                    x.Column(y => y.PlanType);
                    x.Column(y => y.PlanTypeDescription);
                    x.Column(y => y.PlanName);
                    x.Column(y => y.PlanNumber);
                    x.Column(y => y.Elevation);
                    x.Column(y => y.Swing);
                    x.Column(y => y.Stage);
                    x.Column(y => y.BuilderEmployeeId);
                    x.Column(y => y.SalesConsultantEmployeeId);
                    x.Column(y => y.WarrantyExpirationDate);
                    x.Column(y => y.DoNotContact);
                    x.Column(y => y.JdeIdentifier);
                });
        }
    }
}
