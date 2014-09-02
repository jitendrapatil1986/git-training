using NPoco.FluentMappings;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class CommunityAssignmentMap : Map<CommunityAssignment>
    {
        public CommunityAssignmentMap()
        {
            TableName("CommunityAssignments")
                .PrimaryKey(x => x.EmployeeAssignmentId, false)
                .Columns(x =>
                {
                    x.Column(y => y.CommunityId);
                    x.Column(y => y.EmployeeId);
                });
        }
    }
}