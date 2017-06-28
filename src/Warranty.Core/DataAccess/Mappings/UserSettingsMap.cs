using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class UserSettingsMap : AuditableEntityMap<UserSettings>
    {
       public  UserSettingsMap()
        {
            TableName("UserSettings")
               .PrimaryKey(x => x.UserSettingsId, false)
               .Columns(x =>
               {
                   x.Column(col => col.EmployeeId).WithName("EmployeeId");
                   x.Column(col => col.ServiceCallWidgetSize).WithName("ServiceCallWidgetSize");
               });

        }
    }
}
