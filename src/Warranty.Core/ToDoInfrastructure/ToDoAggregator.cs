using System.Linq;
using System.Collections.Generic;
using Common.Security.Queries;
using NPoco;
using Warranty.Core.Enumerations;
using Warranty.Core.Extensions;
using Common.Security.Session;
using Warranty.Core.ToDoInfrastructure.ConcreteTodos;
using Warranty.Core.ToDoInfrastructure.Interfaces;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure
{
    using System;
    using Entities;
    using Services;

    public class ToDoAggregator : IToDoAggregator
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly IServiceCallCreateService _serviceCallCreateService;

        public ToDoAggregator(IDatabase database, IUserSession userSession, IServiceCallCreateService serviceCallCreateService)
        {
            _database = database;
            _userSession = userSession;
            _serviceCallCreateService = serviceCallCreateService;
        }

        public List<IToDo> Execute()
        {
            using (_database)
            {
                var user = _userSession.GetCurrentUser();
                var toDos = new List<IToDo>();

                if (ToDoType.ServiceCallClosure.HasAccess(user.Roles))
                {
                    var serviceCallClosureToDos = GetServiceCallClosureToDos(user, _database);
                    toDos.AddRange(serviceCallClosureToDos);
                }

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
                    var taskJobChangedToDos = GetJobChangedTaskToDos(user, _database, TaskType.JobStage3).ToList();
                    taskJobChangedToDos.AddRange(GetJobChangedTaskToDos(user, _database, TaskType.JobStage7));
                    taskJobChangedToDos.AddRange(GetJobChangedTaskToDos(user, _database, TaskType.JobStage10));

                    //Pull deprecated job task type ToDos for users to complete for those created before the new job task types.
                    taskJobChangedToDos.AddRange(GetJobChangedTaskToDos(user, _database, TaskType.JobStageChanged));
                    taskJobChangedToDos.AddRange(GetJobChangedTaskToDos(user, _database, TaskType.JobClosed));

                    toDos.AddRange(taskJobChangedToDos);
                }
                
                if (ToDoType.JobChangedTaskApproval.HasAccess(user.Roles))
                {
                    var taskJobChangedApprovalToDos = GetStage9JobApprovalToDos(user, _database);

                    toDos.AddRange(taskJobChangedApprovalToDos);
                }

                if (ToDoType.PaymentRequestApprovalUnderWarranty.HasAccess(user.Roles))
                {
                    var paymentRequestApprovalToDos = GetPaymentRequestApprovalUnderWarrantyToDos(user, _database);
                    toDos.AddRange(paymentRequestApprovalToDos);
                }

                if (ToDoType.PaymentRequestApprovalOutOfWarranty.HasAccess(user.Roles))
                {
                    var paymentRequestApprovalToDos = GetPaymentRequestApprovalOutOfWarrantyToDos(user, _database);
                    toDos.AddRange(paymentRequestApprovalToDos);
                }

                if (ToDoType.JobAnniversaryTask.HasAccess(user.Roles))
                {
                    toDos.AddRange(GetTenMonthJobAnniversaryTaskToDos(user, _database, _serviceCallCreateService));
                }

                if (ToDoType.PaymentStatusChanged.HasAccess(user.Roles))
                {
                    toDos.AddRange(GetStatusChangedToDos(user, _database ));
                }

                return toDos.OrderBy(x=>x.Priority).ThenBy(x => x.Date).ToList();
            }
        }

        private IEnumerable<IToDo> GetServiceCallClosureToDos(IUser user, IDatabase database)
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
                                     LEFT OUTER JOIN (SELECT SUM(CASE WHEN ServiceCallLineItemStatusId IN (@0) THEN 1 ELSE 0 END) as NumberOfCompletedLineItems, SUM(CASE WHEN ServiceCallLineItemStatusId IN (@0) THEN 0 ELSE 1 END) as NumberOfIncompleteLineItems, ServiceCallId FROM ServiceCallLineItems GROUP BY ServiceCallId) li
                                       on wc.ServiceCallId = li.ServiceCallId
                                    where 
                                        li.NumberOfIncompleteLineItems = 0
                                    AND wc.ServiceCallStatusId <> @1
                                    AND ci.CityCode in ({0})";
            
            var lineStatusesThatIndicateReadyForClose = new List<int>
            {
                ServiceCallLineItemStatus.Complete.Value,
                ServiceCallLineItemStatus.NoAction.Value
            };

            var query = string.Format(sql, userMarkets.CommaSeparateWrapWithSingleQuote());
            var toDos = database.Fetch<ToDoServiceCallClosure, ToDoServiceCallClosureModel>(query, lineStatusesThatIndicateReadyForClose, ServiceCallStatus.Complete.Value);

            return toDos;
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
                                        ci.CityCode in ({0})
                                    AND wc.ServiceCallType = @1";

            var query = string.Format(sql, userMarkets.CommaSeparateWrapWithSingleQuote());
            var toDos = database.Fetch<ToDoServiceCallApproval, ToDoServiceCallApprovalModel>(query, ServiceCallStatus.Requested.Value, RequestType.WarrantyRequest.DisplayName);

            return toDos;
        }

        private static IEnumerable<IToDo> GetJobChangedTaskToDos(IUser user, IDatabase database, TaskType taskType)
        {
            return GetToDoTasks<ToDoJobStageChangedTask, ToDoJobChangedTaskModel>(user, database, taskType);
        }

        private IEnumerable<IToDo> GetStage9JobApprovalToDos(IUser user, IDatabase database)
        {
            var userMarkets = user.Markets;
            const string sql = @"SELECT t.CreatedDate [Date], Description, TaskId, j.JobId, j.JobNumber
                                FROM Tasks t
                                INNER JOIN Jobs j
                                    ON t.ReferenceId = j.JobId
                                INNER JOIN Communities cm
                                    ON j.CommunityId = cm.CommunityId
                                INNER JOIN Cities c
                                    ON cm.CityId = c.CityId
                                WHERE c.CityCode IN ({0})
                                AND TaskType = @0
                                AND IsComplete = 0";

            var query = string.Format(sql, userMarkets.CommaSeparateWrapWithSingleQuote());
            var toDos = database.Fetch<ToDoJobChangedTaskApproval, ToDoJobChangedTaskApprovalModel>(query, TaskType.JobStage10Approval.Value);

            return toDos;
        }

        private static IEnumerable<IToDo> GetTenMonthJobAnniversaryTaskToDos(IUser user, IDatabase database, IServiceCallCreateService serviceCallCreateService)
        {
            var employeeId = database.ExecuteScalar<Guid>("SELECT EmployeeId FROM Employees where employeeNumber = @0", user.EmployeeNumber);
            if(employeeId!=Guid.Empty)
            {

                var taskType = TaskType.Job10MonthAnniversary;
                const string sqlAnniversaries = @"SELECT j.JobId as ReferenceId, j.JobNumber
                                                FROM Jobs j
                                                INNER JOIN Communities cm
                                                    ON j.CommunityId = cm.CommunityId
                                                INNER JOIN Cities ci
                                                    ON cm.CityId = ci.CityId
                                                INNER JOIN CommunityAssignments ca
                                                    ON cm.CommunityId = ca.CommunityId
                                                INNER JOIN Employees e
                                                    ON ca.EmployeeId = e.EmployeeId
                                                LEFT JOIN Tasks t
                                                    ON j.JobId = t.ReferenceId
                                                AND t.TaskType = @0
                                                WHERE CityCode IN ({0}) 
                                                    AND (MONTH(CloseDate) = MONTH( DATEADD(MM,@1, getdate() )) AND YEAR(CloseDate) = YEAR( DATEADD(MM, @1, getdate())))
                                                    AND e.EmployeeId = @2
                                                    AND t.TaskId IS NULL";

                var sqlNewTasks = string.Format(sqlAnniversaries, user.Markets.CommaSeparateWrapWithSingleQuote());
                var newTasks = database.Fetch<Task>(sqlNewTasks, taskType.Value, -10, employeeId);
                newTasks.ForEach(x =>
                {
                    x.EmployeeId = employeeId;
                    x.Description = taskType.DisplayName;
                    x.TaskType = taskType;
                    database.Insert(x);
                });

                var userMarkets = user.Markets;
                const string sql = @"SELECT DISTINCT
                                        t.TaskId
                                        ,ho.HomeOwnerName
                                        ,ho.HomeOwnerNumber
                                        ,j.AddressLine
                                        ,j.JobId
                                        ,j.JobNumber
                                        ,j.CloseDate as WarrantyStartDate
                                    FROM Tasks t
                                    INNER join Jobs j
                                        ON t.ReferenceId = j.JobId
                                    INNER join HomeOwners ho
                                        ON j.CurrentHomeOwnerId = ho.HomeOwnerId
                                    INNER JOIN Communities cm
                                        ON j.CommunityId = cm.CommunityId
                                    INNER JOIN Cities ci
                                        ON cm.CityId = ci.CityId
                                    WHERE ci.CityCode in ({0})
                                        AND TaskType=@0
                                        AND t.EmployeeId = @1
                                        AND t.IsComplete = 0
                                        AND t.IsNoAction = 0";

                var query = string.Format(sql, userMarkets.CommaSeparateWrapWithSingleQuote());
                var toDos = database.Fetch<ToDoJob10MonthAnniversary, ToDoJob10MonthAnniversaryModel>(query, taskType.Value, employeeId);

                return toDos;
            }

            return new List<ToDoJobAnniversaryTask>();
        }

        private static IEnumerable<TTask> GetToDoTasks<TTask, TModel>(IUser user, IDatabase database, TaskType taskType) where TTask : IToDo where TModel : class
        {
            var query = @"SELECT t.CreatedDate [Date], TaskType, Description, TaskId,  j.JobId, j.JobNumber
                            FROM 
                                [Tasks] t
                            INNER join Employees e
                                ON e.EmployeeId = t.EmployeeId
                            INNER JOIN Jobs j
                                ON t.ReferenceId = j.JobId
                            where 
                                e.EmployeeNumber = @0 and t.TaskType=@1 and t.IsComplete = 0";

            if (taskType == TaskType.JobStage10)
            {
                query = @"SELECT t.CreatedDate [Date], TaskType, Description, TaskId,  j.JobId, j.JobNumber, j.AddressLine Address, j.City, j.StateCode, j.PostalCode, ho.HomeownerName, ho.HomePhone
                            FROM [Tasks] t
                            INNER join Employees e
                                ON e.EmployeeId = t.EmployeeId
                            INNER JOIN Jobs j
                                ON t.ReferenceId = j.JobId
                            LEFT JOIN HomeOwners ho
                                ON j.CurrentHomeOwnerId = ho.HomeOwnerId
                            where e.EmployeeNumber = @0 and t.TaskType=@1 and t.IsComplete = 0";
            }

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
            
            var warrantyEmployees = database.Fetch<Employee>();
            var markets = toDos.Select(x => x.Model.Market).Distinct();

            var employeesByMarket = new List<KeyValuePair<string, List<Employee>>>();
            foreach (var market in markets)
            {
                var securityEmployeesInMarket = new GetUsersByMarketAndRolesQuery(market, Enumerations.UserRoles.WarrantyServiceRepresentativeRole).Execute();
                var employeesInMarket = warrantyEmployees.Where(x => securityEmployeesInMarket.Select(y => y.EmployeeNumber).Contains(x.Number)).ToList();
                employeesByMarket.Add(new KeyValuePair<string, List<Employee>>(market, employeesInMarket));
            }

            toDos.ForEach(
                x =>
                x.Model.Employees = employeesByMarket.Find(y => y.Key == x.Model.Market).Value.Select(securityUser => new ToDoCommunityEmployeeAssignmentModel.EmployeeViewModel
                                                         {
                                                             DisplayName = securityUser.Name,
                                                             EmployeeNumber = securityUser.Number
                                                         }).OrderBy(u => u.DisplayName).ToList());

            return toDos;
        }

        private static IEnumerable<IToDo> GetPaymentRequestApprovalUnderWarrantyToDos(IUser user, IDatabase database)
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
                                WHERE CloseDate >= DATEADD(yy, -2, sc.CreatedDate) {0} /* WHERE */";

            var query = user.IsInRole(Enumerations.UserRoles.WarrantyServiceRepresentative) ? string.Format(sql, "AND c.CityCode IN (" + userMarkets.CommaSeparateWrapWithSingleQuote() + ") AND p.PaymentStatus = " + PaymentStatus.Hold.Value + " AND e.EmployeeNumber = " + user.EmployeeNumber)
                                                                               : string.Format(sql, "AND c.CityCode IN (" + userMarkets.CommaSeparateWrapWithSingleQuote() + ") AND p.PaymentStatus = " + PaymentStatus.Pending.Value);

            var toDos = database.Fetch<ToDoPaymentRequestApprovalUnderWarranty, ToDoPaymentRequestApprovalUnderWarrantyModel>(query);

            return toDos;
        }

        private static IEnumerable<IToDo> GetStatusChangedToDos(IUser user, IDatabase database)
        {
            UpdatePaymentStatusChangedTasks(database, PaymentStatus.Approved, user.EmployeeNumber);
            UpdatePaymentStatusChangedTasks(database, PaymentStatus.Hold, user.EmployeeNumber);

            const string sql = @"SELECT DISTINCT
                                        p.CreatedDate as Date
                                        ,p.PaymentStatus
                                        ,p.Amount
                                        ,t.TaskId
                                        ,t.Description
                                        ,ho.HomeOwnerName
                                        ,ho.HomeOwnerNumber
                                        ,j.AddressLine
                                        ,j.JobId
                                        ,j.JobNumber
                                        ,scli.ServiceCallLineItemId                                    
                                    FROM Tasks t
                                    INNER join Payments p
                                        ON t.ReferenceId = p.PaymentId
                                    INNER JOIN ServiceCallLineItems scli
                                       ON scli.ServiceCallLineItemId = p.ServiceCallLineItemId                                
                                    INNER JOIN ServiceCalls sc
                                       ON sc.ServiceCallId = scli.ServiceCallId
                                    INNER JOIN Jobs j
                                        ON sc.JobId = j.JobId
                                    INNER join HomeOwners ho
                                        ON j.CurrentHomeOwnerId = ho.HomeOwnerId   
                                    INNER JOIN Employees e
                                       ON e.EmployeeId = sc.WarrantyRepresentativeEmployeeId
                                    WHERE e.EmployeeNumber = @0 and t.TaskType= @1 and t.IsComplete = 0";

            var toDos = database.Fetch<ToDoPaymentStatusChanged, ToDoPaymentStatusChangedModel>(sql, user.EmployeeNumber, TaskType.PaymentStatusChanged.Value);

            return toDos;
        }

        private static void UpdatePaymentStatusChangedTasks(IDatabase database, PaymentStatus paymentStatus, string employeeNumber)
        {
            const string sql = @"SELECT p.PaymentId as ReferenceId, e.EmployeeId FROM Payments p
                                    LEFT JOIN Tasks t
                                       ON t.ReferenceId = p.PaymentId and t.TaskType = @0
                                    INNER JOIN ServiceCallLineItems scli
                                       ON scli.ServiceCallLineItemId = p.ServiceCallLineItemId
                                    INNER JOIN ServiceCalls sc
                                       ON sc.ServiceCallId = scli.ServiceCallId
                                    INNER JOIN Jobs j
                                        ON sc.JobId = j.JobId
                                    INNER JOIN Employees e
                                       ON e.EmployeeId = sc.WarrantyRepresentativeEmployeeId
                                    WHERE PaymentStatus = @1 AND e.EmployeeNumber =@2 AND t.TaskId IS NULL;";

            var newTasks = database.Fetch<Task>(sql, TaskType.PaymentStatusChanged.Value, paymentStatus.Value, employeeNumber);
            newTasks.ForEach(x =>
            {
                x.Description = string.Format("Payment Status: {0}", paymentStatus.DisplayName);
                x.TaskType = TaskType.PaymentStatusChanged;
                database.Insert(x);
            });
        }

        private static IEnumerable<IToDo> GetPaymentRequestApprovalOutOfWarrantyToDos(IUser user, IDatabase database)
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
                                WHERE CloseDate < DATEADD(yy, -2, sc.CreatedDate) {0} /* WHERE */";

            var query = user.IsInRole(Enumerations.UserRoles.WarrantyServiceRepresentative) ? string.Format(sql, "AND c.CityCode IN (" + userMarkets.CommaSeparateWrapWithSingleQuote() + ") AND p.PaymentStatus = " + PaymentStatus.Hold.Value + " AND e.EmployeeNumber = " + user.EmployeeNumber)
                                                                               : string.Format(sql, "AND c.CityCode IN (" + userMarkets.CommaSeparateWrapWithSingleQuote() + ") AND p.PaymentStatus = " + PaymentStatus.Pending.Value);

            var toDos = database.Fetch<ToDoPaymentRequestApprovalOutOfWarranty, ToDoPaymentRequestApprovalOutOfWarrantyModel>(query);

            return toDos;
        }
    }
}