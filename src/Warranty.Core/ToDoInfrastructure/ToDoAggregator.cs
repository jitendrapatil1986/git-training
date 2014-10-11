using System.Linq;
using System.Collections.Generic;
using Common.Security.Queries;
using NPoco;
using Warranty.Core.Enumerations;
using Warranty.Core.Extensions;
using Warranty.Core.Security;
using Warranty.Core.ToDoInfrastructure.ConcreteTodos;
using Warranty.Core.ToDoInfrastructure.Interfaces;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure
{
    using Common.Security.Entities;
    using Entities;

    public class ToDoAggregator : IToDoAggregator
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public ToDoAggregator(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public List<IToDo> Execute()
        {
            using (_database)
            {
                var user = _userSession.GetCurrentUser();
                var serviceCallApprovalToDos = GetServiceCallApprovalToDos(user, _database);
                var communityEmployeeAssignmentToDos = GetCommunityEmployeeAssignmentToDos(user, _database);
                //var paymentRequestApprovalToDos = GetPaymentRequestApprovalToDos();

                var toDos = new List<IToDo>();

                toDos.AddRange(serviceCallApprovalToDos);
                toDos.AddRange(communityEmployeeAssignmentToDos);
                //toDos.AddRange(paymentRequestApprovalToDos);

                return toDos.OrderBy(x => x.Date).ToList();
            }
        }

        private static IEnumerable<IToDo> GetServiceCallApprovalToDos(IUser user, IDatabase database)
        {
            var userMarkets = user.Markets;
            const string sql = @"SELECT
                                         wc.CreatedDate as [Date]
                                        ,ho.HomeOwnerName
                                        ,ho.HomeOwnerNumber
                                        ,j.AddressLine
                                        ,wc.ServiceCallId
                                        ,wc.ServiceCallNumber
                                        ,j.JobId
                                        ,j.JobNumber
                                        ,DATEDIFF(yy, j.CloseDate, wc.CreatedDate) as YearsWithinWarranty
                                        ,j.CloseDate as WarrantyStartDate
                                    FROM 
                                        [ServiceCalls] wc
                                    INNER join Jobs j
                                        ON wc.JobId = j.JobId
                                    INNER join HomeOwners ho
                                        ON j.CurrentHomeOwnerId = ho.HomeOwnerId
                                    INNER join Employees e
                                        ON wc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                    INNER JOIN Communities cm
                                        ON j.CommunityId = cm.CommunityId
                                    INNER JOIN Cities ci
                                        ON cm.CityId = ci.CityId
                                    where 
                                        wc.ServiceCallStatusId = @0    
                                    and 
                                        ci.CityCode in ({0})";

            var query = string.Format(sql, userMarkets.CommaSeparateWrapWithSingleQuote());
            var toDos = database.Fetch<ToDoServiceCallApproval, ToDoServiceCallApprovalModel>(query, ServiceCallStatus.Requested.Value);

            return toDos;
        }

        private static IEnumerable<IToDo> GetCommunityEmployeeAssignmentToDos(IUser user, IDatabase database)
        {
            var userMarkets = user.Markets;
            const string sql = @"SELECT c.CreatedDate [Date], c.CommunityId, c.CommunityNumber, c.CommunityName, ci.CityCode as Market
                                FROM Communities c 
                            LEFT JOIN CommunityAssignments ca
                                ON c.CommunityId = ca.CommunityId
                            INNER JOIN Cities ci
                                ON c.CityId = ci.CityId
                            WHERE ci.CityCode IN ({0}) AND ca.EmployeeId IS NULL;";

            var query = string.Format(sql, userMarkets.CommaSeparateWrapWithSingleQuote());
            var toDos = database.Fetch<ToDoCommunityEmployeeAssignment, ToDoCommunityEmployeeAssignmentModel>(query);

            var currentEmployees = database.Fetch<Employee>().Select(x=>x.Number);
            var employeesByMarket = new List<KeyValuePair<string, List<SecurityUser>>>();
            foreach (var market in user.Markets)
            {
                var employeesInMarket = new GetUsersByMarketQuery(market).Execute().Where(x=>currentEmployees.Contains(x.EmployeeNumber)).ToList();
                employeesByMarket.Add(new KeyValuePair<string, List<SecurityUser>>(market, employeesInMarket));
            }

            toDos.ForEach(
                x =>
                x.Model.Employees = employeesByMarket.Find(y => y.Key == x.Model.Market).Value.Select(securityUser => new ToDoCommunityEmployeeAssignmentModel.EmployeeViewModel
                                                         {
                                                             DisplayName = securityUser.DisplayName,
                                                             EmployeeNumber = securityUser.EmployeeNumber
                                                         }).OrderBy(u => u.DisplayName).ToList());

            return toDos;
        }

        //private IEnumerable<IToDo> GetPaymentRequestApprovalToDos()
        //{
        //    //TODO: Not the final query
        //    var todo = new ToDoPaymentRequestApproval()
        //    {
        //        Model = new ToDoPaymentRequestApprovalModel()
        //        {
        //            HomeOwnerAddress = "Address",
        //            HomeOwnerName = "Name",
        //            PaymentAmount = (decimal)150.55
        //        },
        //        Date = DateTime.Now
        //    };

        //    yield return todo;
        //}
    }
}