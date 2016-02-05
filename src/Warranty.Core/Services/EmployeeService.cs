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

            var searchSql = string.Format("WHERE EmployeeNumber LIKE '%{0}'", employeeNumber);

            var allPossibleEmployees = _database.Fetch<Employee>(searchSql);

            var trueMatch = allPossibleEmployees.Where(x => x.Number.Equals(employeeNumber));
            var matches = trueMatch as IList<Employee> ?? trueMatch.ToList();
            if (matches.Count == 1)
            {
                return matches[0];
            }

            foreach (var possibleEmployee in allPossibleEmployees)
            {
                var possibleEmployeeNumber = possibleEmployee.Number;
                if(possibleEmployeeNumber == employeeNumber)
                    return possibleEmployee;
                
                if (string.IsNullOrWhiteSpace(possibleEmployeeNumber))
                    continue;

                possibleEmployeeNumber = RemoveLeadingZeros(possibleEmployeeNumber);

                if (possibleEmployeeNumber != null && employeeNumber.Equals(possibleEmployeeNumber))
                    return possibleEmployee;
            }
            return null;
        }

        private string RemoveLeadingZeros(string numberWithZeros)
        {
            var expression = new Regex(@"[0]*([a-zA-Z0-9]+)$");
            var match = expression.Match(numberWithZeros);
            var groupCount = match.Groups.Count;
            if (groupCount > 0)
            {
                return match.Groups[groupCount - 1].Value;
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