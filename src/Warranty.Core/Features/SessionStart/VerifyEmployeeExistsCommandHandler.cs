namespace Warranty.Core.Features.SessionStart
{
    using Entities;
    using NPoco;
    using Security;

    public class VerifyEmployeeExistsCommandHandler : ICommandHandler<VerifyEmployeeExistsCommand>
    {
        private readonly IUserSession _userSession;
        private readonly IDatabase _database;

        public VerifyEmployeeExistsCommandHandler(IUserSession userSession, IDatabase database)
        {
            _userSession = userSession;
            _database = database;
        }

        public void Handle(VerifyEmployeeExistsCommand message)
        {
            var employee = _userSession.GetCurrentUser();

            using (_database)
            {
                var employeeExists = _database.SingleOrDefault<bool>("Select 1 as bit from Employees where EmployeeNumber=@0", employee.EmployeeNumber);
                if (!employeeExists)
                {
                    var newEmployee = new Employee
                    {
                        Name = string.Format("{0} {1}", employee.FirstName, employee.LastName),
                        Number = employee.EmployeeNumber
                    };
                    _database.Insert(newEmployee);
                }
            }
        }
    }
}
