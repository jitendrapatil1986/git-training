using Common.Security.Session;
using NPoco;
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
                var SqlTemplate = @"SELECT ServiceCallWidgetSize
                                    FROM UserSettings U
                                    INNER JOIN Employees E ON U.EmployeeId = E.EmployeeId
                                    WHERE E.EmployeeId=@0";

                var result = _database.SingleOrDefault<ServiceCallsWidgetModel.UserSettings>(SqlTemplate, emp.EmployeeNumber);

                if (result != null)
                {
                    return result.ServiceCallWidgetSize;
                }
                else
                {
                    return WarrantyConstants.DefaultWidgetSize;
                }
                    
            }
        }
    }
}
