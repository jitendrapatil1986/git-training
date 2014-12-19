namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class VendorPhoneMap : AuditableEntityMap<VendorPhone>
    {
        public VendorPhoneMap()
        {
            TableName("VendorPhones")
                .PrimaryKey("VendorPhoneId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.Type);
                        x.Column(y => y.Number);
                        x.Column(y => y.VendorId);
                    });
        }
    }
}