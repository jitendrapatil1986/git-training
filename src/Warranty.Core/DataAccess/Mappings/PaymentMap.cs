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
                    x.Column(y => y.VendorName);
                    x.Column(y => y.Amount);
                    x.Column(y => y.PaymentStatus);
                    x.Column(y => y.JobNumber);
                    x.Column(y => y.CommunityNumber);
                    x.Column(y => y.JdeIdentifier);
                    x.Column(y => y.InvoiceNumber);
                    x.Column(y => y.Comments);
                    x.Column(y => y.HoldComments);
                    x.Column(y => y.ServiceCallLineItemId);
                    x.Column(y => y.HoldDate);
                    x.Column(y => y.CostCode);
                    x.Column(y => y.ObjectAccount);
                    x.Column(y => y.SendCheckToPC);
                });
        }
    }
}
