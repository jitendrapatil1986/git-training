using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class HomeOwnerMap : AuditableEntityMap<HomeOwner>
    {
        public HomeOwnerMap()
        {
            TableName("HomeOwners")
                .PrimaryKey("HomeOwnerId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.HomeOwnerId);
                        x.Column(y => y.HomeOwnerName);
                        x.Column(y => y.HomeOwnerNumber);
                        x.Column(y => y.HomePhone);
                        x.Column(y => y.JobId);
                        x.Column(y => y.EmailAddress);
                        x.Column(y => y.OtherPhone);
                        x.Column(y => y.WorkPhone1);
                    });
        }
    }
}