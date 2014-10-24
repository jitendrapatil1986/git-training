namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class HomeownerContactMap : AuditableEntityMap<HomeownerContact>
    {
        public HomeownerContactMap()
        {
            TableName("HomeownerContacts")
                .PrimaryKey("HomeownerContactId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.HomeownerId);
                        x.Column(y => y.ContactType);
                        x.Column(y => y.ContactValue);
                    });
        }
    }
}