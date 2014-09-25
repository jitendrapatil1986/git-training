namespace Warranty.Core.Features.ServiceCallSummary.ReassignEmployee
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Security.Queries;
    using NPoco;
    using Entities;

    public class EmployeesForServiceCallQueryHandler : IQueryHandler<EmployeesForServiceCallQuery, List<ServiceCallSummaryModel.EmployeeViewModel>>
    {
        private readonly IDatabase _database;

        public EmployeesForServiceCallQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public List<ServiceCallSummaryModel.EmployeeViewModel> Handle(EmployeesForServiceCallQuery query)
        {
            using (_database)
            {
                const string sql = @"SELECT ci.CityCode 
                                        FROM ServiceCalls sc
                                    INNER JOIN Jobs j
                                        ON sc.JobId = j.JobId
                                    INNER JOIN Communities c
                                        ON j.CommunityId = c.CommunityId
                                    LEFT JOIN CommunityAssignments ca
                                        ON c.CommunityId = ca.CommunityId
                                    INNER JOIN Cities ci
                                        ON c.CityId = ci.CityId
                                    WHERE sc.ServiceCallId = @0;";

                var serviceCallMarket = _database.Single<string>(sql, query.ServiceCallId);

                var warrantyEmployees = _database.Fetch<Employee>().Select(x => x.Number);

                var employeesByServiceCallMarket = new GetUsersByMarketQuery(serviceCallMarket).Execute();

                var employeesAssignableToServiceCall = employeesByServiceCallMarket.Where(x => warrantyEmployees.Contains(x.EmployeeNumber));

                return employeesAssignableToServiceCall.Select(x => new ServiceCallSummaryModel.EmployeeViewModel
                                                                        {
                                                                            DisplayName = x.DisplayName,
                                                                            EmployeeNumber = x.EmployeeNumber
                                                                        }).ToList();
            }
        }
    }
}
