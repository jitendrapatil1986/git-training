using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class WarrantyCallMapping : AuditableEntityMapping<WarrantyCall>
    {
        public WarrantyCallMapping()
        {
            Table("WarrantyCalls");

            Id(x => x.WarrantyCallId);
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