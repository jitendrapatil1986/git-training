using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class CommunityMap : AuditableEntityMap<Community>
    {
        public CommunityMap()
        {
            TableName("Communities")
                .PrimaryKey("CommunityId", false)
                .Columns(x =>
                {
                    x.Column(y => y.CommunityNumber);
                    x.Column(y => y.CommunityName);
                    x.Column(y => y.CityId);
                    x.Column(y => y.DivisionId);
                    x.Column(y => y.ProjectId);
                    x.Column(y => y.SateliteCityId);
                    x.Column(y => y.CommunityStatusCode);
                    x.Column(y => y.CommunityStatusDescription);
                    x.Column(y => y.ProductType);
                    x.Column(y => y.ProductTypeDescription);
                });
        }
    }
}