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
            Property(x => x.IsEscalated, map => map.Column("Escalated"));
            Property(x => x.IsSpecialProject, map => map.Column("SpecialProject"));
            Property(x => x.JobId);
            Property(x => x.Contact);
            Property(x => x.WarrantyRepresentativeEmployeeId);
            Property(x => x.CompletionDate);
            Property(x => x.WorkSummary);
            Property(x => x.HomeOwnerSignature);
        }
    }

    public class ServiceCallMap : AuditableEntityMap<ServiceCall>
    {
        public ServiceCallMap()
        {
            TableName("ServiceCalls")
                .PrimaryKey("ServiceCallId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.ServiceCallNumber);
                        x.Column(y => y.ServiceCallType);
                        x.Column(y => y.ServiceCallStatus).WithName("ServiceCallStatusId");
                        x.Column(y => y.IsEscalated).WithName("Escalated");
                        x.Column(y => y.IsSpecialProject).WithName("SpecialProject");
                        x.Column(y => y.JobId);
                        x.Column(y => y.Contact);
                        x.Column(y => y.WarrantyRepresentativeEmployeeId);
                        x.Column(y => y.CompletionDate);
                        x.Column(y => y.WorkSummary);
                        x.Column(y => y.HomeOwnerSignature);
                    });
        }
    }
}