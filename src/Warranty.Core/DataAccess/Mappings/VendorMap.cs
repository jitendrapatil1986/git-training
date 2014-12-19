namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class VendorMap : AuditableEntityMap<Vendor>
    {
        public VendorMap()
        {
            TableName("Vendors")
                .PrimaryKey("VendorId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.Name);
                        x.Column(y => y.Number);
                    });
        }
    }
}