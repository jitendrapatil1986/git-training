using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class EntityMapper : NPoco.FluentMappings.Mappings
    {
        public EntityMapper()
        {
            For<Payment>().TableName("Payments")
                .PrimaryKey("PaymentId", false)
                .Columns(x =>
                {
                    x.Column(y => y.VendorNumber);
                    x.Column(y => y.Amount);
                    x.Column(y => y.PaymentStatus);
                    x.Column(y => y.JobNumber);
                    x.Column(y => y.JdeIdentifier);
                });

            For<IAuditableEntity>()
                .Columns(x =>
                {
                    x.Column(y => y.CreatedDate);
                    x.Column(y => y.CreatedBy);
                    x.Column(y => y.UpdatedDate);
                    x.Column(y => y.UpdatedBy);
                });
        }
    }
}
