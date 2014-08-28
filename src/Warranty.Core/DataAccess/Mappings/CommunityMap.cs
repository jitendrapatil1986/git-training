namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class CommunityMap : AuditableEntityMap<Community>
    {
        public CommunityMap()
        {
            TableName("Communities");
            PrimaryKey(x => x.CommunityId, false);
            Columns(x =>
                        {
                            x.Column(col => col.CommunityName);
                            x.Column(col => col.CityId);
                            x.Column(col => col.CommunityNumber);
                            x.Column(col => col.CommunityStatusCode);
                            x.Column(col => col.CommunityStatusDescription);
                            x.Column(col => col.DivisionId);
                            x.Column(col => col.ProductType);
                            x.Column(col => col.ProductTypeDescription);
                            x.Column(col => col.ProjectId);
                            x.Column(col => col.SateliteCityId);
                        });
        }
    }
}