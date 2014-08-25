namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;
    /*
    public class JobMapping : AuditableEntityMapping<Job>
    {
        public JobMapping()
        {
            Table("Jobs");

            Id(x => x.JobId, map => map.Generator(Generators.GuidComb));
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
            Property(x => x.BuilderEmployeeId);
            Property(x => x.SalesConsultantEmployeeId);
            Property(x => x.WarrantyExpirationDate);
            Property(x => x.TotalPrice);
            Property(x => x.DoNotContact);
            Property(x => x.JdeIdentifier);
        }
    }
     * */
}