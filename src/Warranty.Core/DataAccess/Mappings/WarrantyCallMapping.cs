namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class WarrantyCallMapping : AuditableEntityMapping<WarrantyCall>
    {
        public WarrantyCallMapping()
        {
            Table("WarrantyCalls");

            Id(x => x.WarrantyCallId, map => map.Generator(new GuidCombGeneratorDef()));
            Property(x => x.WarrantyCallNumber);
            Property(x => x.WarrantyCallType);
            Property(x => x.JobId);
            Property(x => x.Contact);
            Property(x => x.WarrantyRepresentativeEmployeeId);
            Property(x => x.CompletionDate);
            Property(x => x.WorkSummary);
            Property(x => x.HomeOwnerSignature);
        }
    }
}