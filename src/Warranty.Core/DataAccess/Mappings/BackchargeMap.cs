namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class BackchargeMap : AuditableEntityMap<Backcharge>
    {
        public BackchargeMap()
        {
            TableName("Backcharges")
                .PrimaryKey("BackchargeId", false)
                .Columns(x =>
                {
                    x.Column(y => y.BackchargeAmount);
                    x.Column(y => y.BackchargeReason);
                    x.Column(y => y.BackchargeResponseFromVendor);
                    x.Column(y => y.BackchargeVendorNumber);
                    x.Column(y => y.BackchargeVendorName);
                    x.Column(y => y.PaymentId);
                    x.Column(y => y.PersonNotified);
                    x.Column(y => y.PersonNotifiedDate);
                    x.Column(y => y.PersonNotifiedPhoneNumber);
                    x.Column(y => y.JdeIdentifier);
                    x.Column(y => y.HoldComments);
                    x.Column(y => y.DenyComments);
                    x.Column(y => y.BackchargeStatus);
                    x.Column(y => y.CostCode);
                    x.Column(y => y.HoldDate);
                    x.Column(y => y.DenyDate);
                    x.Column(y => y.Username);
                    x.Column(y => y.EmployeeNumber);
                    x.Column(y => y.JobNumber);
                    x.Column(y => y.ServiceCallLineItemId);
                    x.Column(y => y.ObjectAccount);
                });
        }
    }
}