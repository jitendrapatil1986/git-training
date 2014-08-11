using NHibernate.Mapping.ByCode;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class PaymentMapping : AuditableEntityMapping<Payment>
    {
        public PaymentMapping()
        {
            Table("Payments");

            Id(x => x.PaymentId, map => map.Generator(Generators.GuidComb));
            Property(x => x.VendorNumber);
            Property(x => x.Amount);
            Property(x => x.PaymentStatus);
            Property(x => x.JobNumber);
            Property(x => x.JdeIdentifier);
        }
    }
}