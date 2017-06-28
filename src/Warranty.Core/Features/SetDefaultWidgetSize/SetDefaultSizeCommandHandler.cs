
namespace Warranty.Core.Features.SetDefaultWidgetSize
{
    using System;
    using Entities;   
    using NPoco;
   
    using Common.Security.Session;
    using NServiceBus;
    
    public class SetDefaultSizeCommandHandler: ICommandHandler<SetDefaultWidgetSizeCommand, UserSettings>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;
        public string Employeeid;
        public SetDefaultSizeCommandHandler(IDatabase database, IBus bus, IUserSession userSession)
        {
            _database = database;
            _bus = bus;
            _userSession = userSession;
        }

        public UserSettings Handle(SetDefaultWidgetSizeCommand message)
        {
            var emp = _userSession.GetCurrentUser();
            using (_database)
            {
                string SqlTemplate = @"Select EmployeeId from Employees where EmployeeNumber= '" + emp.EmployeeNumber + "'";
                var sql = string.Format(SqlTemplate, "where EmployeNumber= @0");
                
                var employee = _database.FirstOrDefault<Employee>("Where EmployeeNumber=@0", emp.EmployeeNumber);

                var DefaultWidget = _database.FirstOrDefault<UserSettings>("Where EmployeeId=@0", employee.EmployeeId);
                if (DefaultWidget != null)
                {

                    DefaultWidget.ServiceCallWidgetSize = message.ServiceCallWidgetSize;
                    DefaultWidget.UpdatedDate = DateTime.Now;
                    _database.Update(DefaultWidget);

                    return DefaultWidget;
                
                }
                else
                {
                    var DefaultWidgetSize = new UserSettings
                    {

                        EmployeeId = employee.EmployeeId,
                        ServiceCallWidgetSize = message.ServiceCallWidgetSize,
                        UpdatedDate = DateTime.Now,
                    };
                    _database.Insert(DefaultWidgetSize);
                    return DefaultWidgetSize;
                    
                }
            }                     
        }
    }
}
