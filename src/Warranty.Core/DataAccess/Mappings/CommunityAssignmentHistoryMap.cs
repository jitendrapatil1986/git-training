using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class CommunityAssignmentHistoryMap : AuditCreatedEntityMap<CommunityAssignmentHistory>
    {
        public CommunityAssignmentHistoryMap()
        {
            TableName("CommunityAssignmentHistory")
                .PrimaryKey("EmployeeAssignmentHistoryId", false)
                .Columns(x =>
                {
                    x.Column(y => y.CommunityId);
                    x.Column(y => y.EmployeeId);
                });
        }
    }
}
