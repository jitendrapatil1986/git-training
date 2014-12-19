namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class VendorEmailMap : AuditableEntityMap<VendorEmail>
    {
        public VendorEmailMap()
        {
            TableName("VendorEmails")
                .PrimaryKey("VendorEmailId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.Email);
                        x.Column(y => y.VendorId);
                    });
        }
    }
}