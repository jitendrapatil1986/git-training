using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Security.Session;
using NPoco;
using StructureMap.Query;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Extensions;

namespace Warranty.Core.Features.Report.WSROpenActivity
{
    public class WSROpenActivityQueryHandler : IQueryHandler<WSROpenActivityQuery, WSROpenActivityModel>
    {
        private readonly IDatabase _database;
        private readonly IUser _user;

        public WSROpenActivityQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _user = userSession.GetCurrentUser();
        }

        public WSROpenActivityModel Handle(WSROpenActivityQuery query)
        {
            var model = query.Model;

            if (!(_user.IsInRole(UserRoles.WarrantyServiceCoordinator) || _user.IsInRole(UserRoles.CustomerCareManager)))
            {
                var employeeId = GetCurrentUserId(_user.EmployeeNumber);

                if (!employeeId.HasValue) // contractors won't have a value - so there's no point in continuing
                    return model;

                model.TeamMemberId = employeeId;
            }

            var activities = new List<OpenActivity>();
            var serviceCalls = GetServiceCalls(model);

            foreach (var employeeCalls in serviceCalls.GroupBy(e => e.EmployeeId))
            {
                var id = employeeCalls.Key;
                var name = "n\\a";
                if (employeeCalls.FirstOrDefault() != null)
                {
                    name = employeeCalls.FirstOrDefault().EmployeeName;
                }

                var openTasks = GetOpenTasks(id);

                var activity = new OpenActivity
                {
                    EmployeeName = name,
                    ServiceCalls = employeeCalls,
                    OpenTasks = openTasks
                };
                activities.Add(activity);
            }

            model.OpenActivities = activities;
            return model;
        }

        private Guid? GetCurrentUserId(string employeeNumber)
        {
            return _database.SingleOrDefault<Guid?>("SELECT EmployeeId FROM Employees WHERE EmployeeNumber = @0", employeeNumber);
        }

        private IEnumerable<OpenTask> GetOpenTasks(Guid employeeId)
        {
            var tasks = _database.Fetch<OpenTask>(@"SELECT t.*, j.JobNumber
                                                    FROM Tasks t
                                                    LEFT JOIN Jobs j ON j.JobId = t.ReferenceId
                                                    WHERE t.EmployeeId = @0 
                                                        AND t.IsComplete = 0 
                                                    ORDER BY CreatedDate",
                                                    employeeId);

            if(tasks == null)
                return new List<OpenTask>();

            return tasks.Where(t => t.TaskType.Stage.HasValue);
        }

        private IEnumerable<ServiceCall> GetServiceCalls(WSROpenActivityModel model)
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT 
                            sc.ServiceCallId as ServiceCallId
                            , sc.WarrantyRepresentativeEmployeeId As EmployeeId
                            , e.EmployeeName
                            , Servicecallnumber as CallNumber
                            , sc.CreatedDate
                            , sc.SpecialProject as IsSpecialProject
                            , sc.JobId
                            , j.AddressLine as [Address]
                            , j.City 
                            , j.StateCode
                            , j.PostalCode
                            , j.JobNumber
                            , ho.HomeOwnerName
                            , ho.HomePhone
                            , ho.OtherPhone
                            , cm.CommunityName
                            , li.LineNumber
                            , li.ProblemCode
                            , li.ProblemDescription
                            , j.CommunityId
                        FROM ServiceCalls sc
	                        INNER JOIN Jobs j ON sc.JobId = j.JobId
	                        INNER JOIN HomeOwners ho ON j.CurrentHomeOwnerId = ho.HomeOwnerId
	                        LEFT JOIN ServiceCallLineItems li ON sc.ServiceCallId = li.ServiceCallId
	                        LEFT JOIN Employees e ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
	                        INNER JOIN Communities cm ON j.CommunityId = cm.CommunityId
                            INNER JOIN Cities c on cm.CityId = c.CityId AND c.CityCode IN(@1)
                        WHERE sc.ServiceCallStatusId = @0");

            if (model.TeamMemberId.HasValue)
                sql.Append(" AND sc.WarrantyRepresentativeEmployeeId = @2");
            else if(model.ProjectId.HasValue)
                sql.Append(" AND cm.ProjectId = @2");
            else if (model.DivisionId.HasValue)
                sql.Append(" AND cm.DivisionId = @2");

            sql.Append(" ORDER BY cm.CommunityName, ho.HomeOwnerName, j.AddressLine");

            var markets = _user.Markets;

            var serviceCalls = _database.FetchOneToMany<ServiceCall, ServiceCallLine>(x => 
                                                        x.ServiceCallId, 
                                                        sql.ToString(),
                                                        ServiceCallStatus.Open.Value, // @0
                                                        markets,                      // @1
                                                        model.FilterValue);           // @2

            foreach (var serviceCall in serviceCalls)
            {
                const string notesSql = @"SELECT ServiceCallNote AS Note FROM ServiceCallNotes WHERE ServiceCallId = @0 AND (ServiceCallNote <> '' OR ServiceCallNote != null)";

                serviceCall.ServiceCallNotes = _database.Fetch<ServiceCallNote>(notesSql, serviceCall.ServiceCallId);
            }

            return serviceCalls;

        }
    }
}