using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Warranty.Core.Services
{
    using Entities;
    using Extensions;
    using NPoco;
    using Common.Security.Session;

    public class EmployeeService : IEmployeeService
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public EmployeeService(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public Employee GetEmployeeByNumber(int? employeeNumber)
        {
            if (!employeeNumber.HasValue)
                return null;

            return GetEmployeeByNumber(employeeNumber.Value.ToString());
        }

        public Employee GetEmployeeByNumber(string employeeNumber)
        {
            if (string.IsNullOrWhiteSpace(employeeNumber))
                return null;
            var searchSql = "";
            int intEmployeeNumber;
            if (int.TryParse(employeeNumber, out intEmployeeNumber))
            {
                searchSql = string.Format("WHERE ISNUMERIC(EmployeeNumber) = 1 AND CAST(EmployeeNumber AS INT) = {0}", intEmployeeNumber);
            }
            else
            {
                searchSql = string.Format(@"WHERE EmployeeNumber = '{0}'", employeeNumber);
            }

            var allPossibleEmployees = _database.Fetch<Employee>(searchSql);
            if (allPossibleEmployees.Count == 1)
            {
                return allPossibleEmployees[0];
            }

            var trueMatch = allPossibleEmployees.Where(x => x.Number == employeeNumber).ToList();
            if (trueMatch.Count == 1)
            {
                return trueMatch[0];
            }
            return null;
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