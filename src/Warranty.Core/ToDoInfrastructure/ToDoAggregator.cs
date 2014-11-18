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
    using System;
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
                var toDos = new List<IToDo>();

                if (ToDoType.ServiceCallApproval.HasAccess(user.Roles))
                {
                    var serviceCallApprovalToDos = GetServiceCallApprovalToDos(user, _database);
                    toDos.AddRange(serviceCallApprovalToDos);
                }

                if (ToDoType.CommunityEmployeeAssignment.HasAccess(user.Roles))
                {
                    var communityEmployeeAssignmentToDos = GetCommunityEmployeeAssignmentToDos(user, _database);
                    toDos.AddRange(communityEmployeeAssignmentToDos);
                }

                if (ToDoType.JobChangedTask.HasAccess(user.Roles))
                {
                    var taskJobChangedToDos = GetJobChangedTaskToDos(user, _database, TaskType.JobStageChanged).ToList();
                    taskJobChangedToDos.AddRange(GetJobChangedTaskToDos(user, _database, TaskType.JobClosed));
                    toDos.AddRange(taskJobChangedToDos);
                }

                if (ToDoType.PaymentRequestApproval.HasAccess(user.Roles))
                {
                    var paymentRequestApprovalToDos = GetPaymentRequestApprovalToDos(user, _database);
                    toDos.AddRange(paymentRequestApprovalToDos);
                }
                
                if (ToDoType.JobAnniversaryTask.HasAccess(user.Roles))
                {
                    
                    toDos.AddRange(GetJobAnniversaryTaskToDos(user, _database, TaskType.Job3MonthAnniversary, 3));
                    toDos.AddRange(GetJobAnniversaryTaskToDos(user, _database, TaskType.Job5MonthAnniversary, 5));
                    toDos.AddRange(GetJobAnniversaryTaskToDos(user, _database, TaskType.Job9MonthAnniversary, 9));
                }

                return toDos.OrderBy(x=>x.Priority).ThenBy(x => x.Date).ToList();
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

        private static IEnumerable<IToDo> GetJobChangedTaskToDos(IUser user, IDatabase database, TaskType taskType)
        {
            return GetToDoTasks<ToDoJobStageChangedTask, ToDoJobChangedTaskModel>(user, database, taskType);
        }

        private static IEnumerable<IToDo> GetJobAnniversaryTaskToDos(IUser user, IDatabase database, TaskType taskType, int months)
        {
            var employeeId = database.ExecuteScalar<Guid>("SELECT EmployeeId FROM Employees where employeeNumber = @0", user.EmployeeNumber);
            
            const string sqlAnniversaries = @"SELECT j.JobId as ReferenceId, j.JobNumber
                                                FROM Jobs j                                    
                                                    INNER JOIN Communities cm
                                                        ON j.CommunityId = cm.CommunityId
                                                    INNER JOIN Cities ci
                                                        ON cm.CityId = ci.CityId
                                                    LEFT JOIN Tasks t
                                                        ON j.JobId = t.ReferenceId
                                                    AND t.TaskType = @0
                                                    WHERE CityCode IN ({0}) 
                                                        AND (MONTH(CloseDate) = MONTH( DATEADD(MM,@1, getdate() )) AND YEAR(CloseDate) = YEAR( DATEADD(MM, @1, getdate())))
                                                        AND t.TaskId IS NULL";
            
            var sqlNewTasks = string.Format(sqlAnniversaries, user.Markets.CommaSeparateWrapWithSingleQuote());
            var newTasks = database.Fetch<Task>(sqlNewTasks, taskType.Value, -months);
            newTasks.ForEach(x =>
                {
                    x.EmployeeId = employeeId;
                    x.Description = taskType.DisplayName;
                    x.TaskType = taskType;
                    database.Insert(x);
                });

            var toDos = GetToDoTasks<ToDoJobAnniversaryTask, ToDoJobAnniversaryTaskModel>(user, database, taskType);

            toDos.ForEach(x =>
                {
                    x.Model.NumberOfMonths = months;
                });

            return toDos;
        }

        private static IEnumerable<TTask> GetToDoTasks<TTask, TModel>(IUser user, IDatabase database, TaskType taskType) where TTask : IToDo where TModel : class
        {
            const string query = @"SELECT t.CreatedDate [Date], Description, TaskId,  j.JobId, j.JobNumber
                                    FROM 
                                        [Tasks] t
                                    INNER join Employees e
                                        ON e.EmployeeId = t.EmployeeId
                                    INNER JOIN Jobs j
                                        ON t.ReferenceId = j.JobId
                                    where 
                                        e.EmployeeNumber = @0 and t.TaskType=@1 and t.IsComplete = 0";

            var toDos = database.Fetch<TTask, TModel>(query, user.EmployeeNumber, taskType.Value);

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

        private static IEnumerable<IToDo> GetPaymentRequestApprovalToDos(IUser user, IDatabase database)
        {
            var userMarkets = user.Markets;
            
            const string sql = @"SELECT p.PaymentId, sc.ServiceCallId, sc.ServiceCallNumber, li.ServiceCallLineItemId, p.VendorName, p.InvoiceNumber, p.Amount, 
                                        p.PaymentStatus, p.HoldComments, p.UpdatedBy, p.UpdatedDate FROM Payments p
                                INNER JOIN ServiceCallLineItems li
                                ON p.ServiceCallLineItemId = li.ServiceCallLineItemId
                                INNER JOIN ServiceCalls sc
                                ON li.ServiceCallId = sc.ServiceCallId
                                INNER JOIN Jobs j
                                ON p.JobNumber = j.JobNumber
                                INNER JOIN Communities cm
                                ON j.CommunityId = cm.CommunityId
                                INNER JOIN Cities c
                                ON cm.CityId = c.CityId
                                INNER JOIN Employees e
                                ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                {0} /* WHERE */";

            var query = user.IsInRole(UserRoles.WarrantyServiceRepresentative) ? string.Format(sql, "WHERE c.CityCode IN (" + userMarkets.CommaSeparateWrapWithSingleQuote() + ") AND p.PaymentStatus = " + PaymentStatus.Hold.Value + " AND e.EmployeeNumber = " + user.EmployeeNumber) 
                                                                               : string.Format(sql, "WHERE c.CityCode IN (" + userMarkets.CommaSeparateWrapWithSingleQuote() + ") AND p.PaymentStatus = " + PaymentStatus.Pending.Value);
            
            var toDos = database.Fetch<ToDoPaymentRequestApproval, ToDoPaymentRequestApprovalModel>(query);

            return toDos;
        }
    }
}