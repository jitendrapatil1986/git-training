namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class ServiceCallMapping : AuditableEntityMapping<ServiceCall>
    {
        public ServiceCallMapping()
        {
            Table("ServiceCalls");

            Id(x => x.ServiceCallId, map => map.Generator(Generators.GuidComb));
            Property(x => x.ServiceCallNumber);
            Property(x => x.ServiceCallType);
            Property(x => x.ServiceCallStatus);
            Property(x => x.IsSpecialProject, map => map.Column("SpecialProject"));
            Property(x => x.JobId);
            Property(x => x.Contact);
            Property(x => x.WarrantyRepresentativeEmployeeId);
            Property(x => x.CompletionDate);
            Property(x => x.WorkSummary);
            Property(x => x.HomeOwnerSignature);
        }
    }
}