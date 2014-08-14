using NPoco.FluentMappings;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class PaymentMapping : Map<Payment>
    {
        public PaymentMapping()
        {
            PrimaryKey("PaymentId");
            TableName("Payments");

            Columns(x =>
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