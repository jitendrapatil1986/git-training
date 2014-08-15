namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class PaymentMap : AuditableEntityMap<Payment>
    {
        public PaymentMap()
        {
            TableName("Payments")
                .PrimaryKey("PaymentId", false)
                .Columns(x =>
                {
                    x.Column(y => y.VendorNumber);
                    x.Column(y => y.Amount);
                    x.Column(y => y.PaymentStatus);
                    x.Column(y => y.JobNumber);
                    x.Column(y => y.JdeIdentifier);
                });
        }
    }
}
