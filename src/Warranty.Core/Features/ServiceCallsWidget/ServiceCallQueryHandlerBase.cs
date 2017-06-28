using Common.Security.Session;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warranty.Core.Configurations;

namespace Warranty.Core.Features.ServiceCallsWidget
{
    public class ServiceCallQueryHandlerBase 
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        public ServiceCallQueryHandlerBase(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }
        public int GetServiceCallWidgetSize(IUser user)
        {
            var emp = _userSession.GetCurrentUser();
            using (_database)
            {

                var SqlTemplate = @"SELECT Top 1 ServiceCallWidgetSize, max(U.UpdatedDate) as UpdatedDate from UserSettings U
                                               INNER JOIN Employees E
                                               ON U.EmployeeId = E.EmployeeId
											   Where Exists(Select EmployeeId from Employees where E.EmployeeNumber = @0) 
                                                Group by ServiceCallWidgetSize, U.UpdatedDate 
                                                Order by U.UpdatedDate DESC";

                var result = _database.SingleOrDefault<ServiceCallsWidgetModel.UserSettings>(SqlTemplate, emp.EmployeeNumber);

                if (result != null)
                {
                    return result.ServiceCallWidgetSize;
                }
                else
                    return WarrantyConstants.DefaultWidgetSize;
            }
        }
    }
}
