using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Security.Session;
using NPoco;
using StructureMap.Query;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Features.Report.WSROpenActivity
{
    public class WSROpenActivityQueryHandler : IQueryHandler<WSROpenActivityQuery, WSROpenActivityModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WSROpenActivityQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public WSROpenActivityModel Handle(WSROpenActivityQuery query)
        {
            var model = query.Model;
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

        private IEnumerable<OpenTask> GetOpenTasks(Guid employeeId)
        {
            var tasks = _database.Fetch<Task>("WHERE EmployeeId = @0 AND IsComplete = 0 ORDER BY CreatedDate", employeeId);
            if(tasks == null || !tasks.Any())
                return new List<OpenTask>();

            return tasks.Select(t =>
                new OpenTask
                {
                    Task = t.TaskType.DisplayName,
                    Description = t.Description,
                    CreateDate = t.CreatedDate
                });
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
                            , j.AddressLine as [Address]
                            , j.City 
                            , j.StateCode
                            , j.PostalCode
                            , ho.HomeOwnerName
                            , ho.HomePhone
                            , ho.OtherPhone
                            , cm.CommunityName
                            , li.LineNumber
                            , li.ProblemCode
                            , li.ProblemDescription
                        FROM ServiceCalls sc
	                        INNER JOIN Jobs j ON sc.JobId = j.JobId
	                        INNER JOIN HomeOwners ho ON j.CurrentHomeOwnerId = ho.HomeOwnerId
	                        LEFT JOIN ServiceCallLineItems li ON sc.ServiceCallId = li.ServiceCallId
	                        LEFT JOIN Employees e ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
	                        INNER JOIN Communities cm ON j.CommunityId = cm.CommunityId
                        WHERE sc.ServiceCallStatusId = @0");

            var divisions = new List<Division>();

            if (model.TeamMemberId.HasValue)
                sql.Append(" AND sc.WarrantyRepresentativeEmployeeId = @1");
            else if(model.ProjectId.HasValue)
                sql.Append(" AND cm.ProjectId = @1");
            else if (model.DivisionId.HasValue)
                sql.Append(" AND cm.DivisionId = @1");
            else
            {
                var markets = _userSession.GetCurrentUser().Markets;

                const string divisonSql = @"SELECT DISTINCT
	                        d.DivisionId,
	                        d.DivisionName,
	                        d.DivisionCode
                        FROM 
	                        Communities com
	                        LEFT JOIN Cities c ON c.CityId = com.CityId
	                        LEFT JOIN Divisions d on d.DivisionId = com.DivisionId
                        WHERE d.DivisionId IS NOT NULL AND c.CityCode IN (@0)";

                divisions = _database.Fetch<Division>(divisonSql, markets);
                sql.Append(" AND cm.DivisionId IN(@2)");
            }

            sql.Append(" ORDER BY cm.CommunityName, ho.HomeOwnerName, j.AddressLine");

            var serviceCalls = _database.FetchOneToMany<ServiceCall, ServiceCallLine>(x => 
                                                        x.ServiceCallId, 
                                                        sql.ToString(),
                                                        ServiceCallStatus.Open.Value,
                                                        model.FilterValue, 
                                                        divisions.Select(d => d.DivisionId));

            foreach (var serviceCall in serviceCalls)
            {
                const string notesSql = @"SELECT ServiceCallNote AS Note FROM ServiceCallNotes WHERE ServiceCallId = @0 AND (ServiceCallNote <> '' OR ServiceCallNote != null)";

                serviceCall.ServiceCallNotes = _database.Fetch<ServiceCallNote>(notesSql, serviceCall.ServiceCallId);
            }

            return serviceCalls;

        }
    }
}