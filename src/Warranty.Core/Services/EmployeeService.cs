namespace Warranty.Core.Services
{
    using Extensions;
    using NPoco;
    using Security;

    public class EmployeeService : IEmployeeService
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public EmployeeService(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public string[] GetEmployeesInMarket()
        {
            var markets = GetEmployeeMarkets();
            using (_database)
            {
                var sql = @"SELECT DISTINCT EmployeeNumber
                            FROM Employees e
                            INNER JOIN CommunityAssignments ca
                            ON e.EmployeeId = ca.EmployeeId
                            INNER JOIN Communities c
                            ON ca.CommunityId = c.CommunityId
                            INNER JOIN Cities ci
                            ON c.CityId = ci.CityId
                            WHERE CityCode IN ({0})";

                var employeesInMarket = _database.Fetch<string>(string.Format(sql, markets));
                return employeesInMarket.ToArray();
            }
        }

        public string GetEmployeeMarkets()
        {
            return _userSession.GetCurrentUser().Markets.CommaSeparateWrapWithSingleQuote();
        }
    }
}