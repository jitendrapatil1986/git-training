﻿namespace Warranty.Core.DataAccess.Mappings
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
                    x.Column(y => y.PaymentId);
                    x.Column(y => y.PersonNotified);
                    x.Column(y => y.PersonNotifiedDate);
                    x.Column(y => y.PersonNotifiedPhoneNumber);
                });
        }
    }
}