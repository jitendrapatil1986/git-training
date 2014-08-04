using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class JobMapping : ClassMapping<Job>
    {
        public JobMapping()
        {
            Table("Jobs");

            Id(x => x.JobId);
            Property(x => x.JobNumber);
            Property(x => x.CloseDate);
            Property(x => x.AddressLine);
            Property(x => x.City);
            Property(x => x.StateCode);
            Property(x => x.PostalCode);
            Property(x => x.LegalDescription);
            Property(x => x.CommunityId);
            Property(x => x.CurrentHomeOwnerId);
            Property(x => x.PlanType);
            Property(x => x.PlanTypeDescription);
            Property(x => x.PlanName);
            Property(x => x.PlanNumber);
            Property(x => x.Elevation);
            Property(x => x.Swing);
            Property(x => x.BuilderId);
            Property(x => x.SalesConsultantId);
            Property(x => x.WarrantyExpirationDate);
            Property(x => x.TotalPrice);
            Property(x => x.DoNotContact);
            Property(x => x.CreatedDate);
            Property(x => x.CreatedBy);
            Property(x => x.UpdatedDate);
            Property(x => x.UpdatedBy);
        }
    }
}