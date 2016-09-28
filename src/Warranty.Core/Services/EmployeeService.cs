using System;
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
        private ICommunityService _communityService;

        public EmployeeService(IDatabase database, IUserSession userSession, ICommunityService communityService)
        {
            _database = database;
            _userSession = userSession;
            _communityService = communityService;
        }

        public Employee GetEmployeeByNumber(int? employeeNumber)
        {
            if (!employeeNumber.HasValue)
                return null;

            return GetEmployeeByNumber(employeeNumber.Value.ToString().Substring(2));
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

        public Employee GetWsrByJobId(Guid jobId)
        {
            var sql = string.Format(@"select 
                                            e.EmployeeId
                                            ,e.EmployeeNumber
                                            ,e.EmployeeName
                                            ,e.CreatedDate
                                            ,e.CreatedBy
                                            ,e.UpdatedDate
                                            ,e.UpdatedBy
                                        from jobs j
                                            join Communities c on c.CommunityId = j.CommunityId
                                            join CommunityAssignments ca on ca.CommunityId = c.CommunityId
                                            join Employees e on e.EmployeeId = ca.EmployeeId
                                        where j.JobId = '{0}'", jobId);
            using (_database)
            {
                return _database.SingleOrDefault<Employee>(sql);
            }
        }

        public Employee GetWsrByCommunity(string communityNumber)
        {
            if (string.IsNullOrWhiteSpace(communityNumber))
                throw new ArgumentNullException("communityNumber");

            var selectSql = @"SELECT Employees.EmployeeId
                                  ,Employees.EmployeeNumber
                                  ,Employees.EmployeeName
                                  ,Employees.CreatedDate
                                  ,Employees.CreatedBy
                                  ,Employees.UpdatedDate
                                  ,Employees.UpdatedBy
                              FROM CommunityAssignments Assignments
                              JOIN Communities Community on Community.CommunityId = Assignments.CommunityId
                              JOIN Employees Employees on Employees.EmployeeId = Assignments.EmployeeId
                              WHERE CommunityNumber = @0";

            return _database.SingleOrDefault<Employee>(selectSql, _communityService.GetWarrantyCommunityNumber(communityNumber));
        }

        public string GetEmployeeMarkets()
        {
            return _userSession.GetCurrentUser().Markets.CommaSeparateWrapWithSingleQuote();
        }
    }
}