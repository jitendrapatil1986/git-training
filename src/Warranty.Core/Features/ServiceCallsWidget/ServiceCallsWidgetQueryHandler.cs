namespace Warranty.Core.Features.ServiceCallsWidget
{
    using System.Collections.Generic;
    using Enumerations;
    using NPoco;
    using Security;
    using Extensions;

    public class ServiceCallsWidgetQueryHandler : IQueryHandler<ServiceCallsWidgetQuery, ServiceCallsWidgetModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public ServiceCallsWidgetQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public ServiceCallsWidgetModel Handle(ServiceCallsWidgetQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                return new ServiceCallsWidgetModel
                           {
                               MyServiceCalls = GetMyServiceCalls(user),
                               OpenServiceCalls = GetOpenServiceCalls(user),
                               SpecialProjectServiceCalls = GetSpecialProjects(user),
                               EscalatedServiceCalls = GetEscalatedServiceCalls(user),
                           };
            }
        }

        const string SqlTemplate = @"SELECT 
                                          wc.ServiceCallId as ServiceCallId
                                        , j.JobId
                                        , Servicecallnumber as CallNumber
                                        , j.AddressLine as [Address]
                                        , wc.CreatedDate 
                                        , ho.HomeOwnerName
                                        , ho.HomeOwnerNumber
                                        , NumberOfLineItems
                                        , ho.HomePhone as PhoneNumber
                                        , e.EmployeeName as AssignedTo
                                        , e.EmployeeNumber as AssignedToEmployeeNumber
                                        , wc.EscalationDate
                                        , wc.EscalationReason
                                        , DATEDIFF(yy, j.CloseDate, wc.CreatedDate) as YearsWithinWarranty
                                        , j.CloseDate as WarrantyStartDate
                                        , j.JobNumber
                                        , wc.CompletionDate
                                        , wc.SpecialProject as IsSpecialProject
                                        , wc.Escalated as IsEscalated
                                        , DATEDIFF(dd, wc.CreatedDate, wc.CompletionDate) as DaysOpenedFor
                                     FROM [ServiceCalls] wc
                                     inner join Jobs j
                                       on wc.JobId = j.JobId
                                     inner join HomeOwners ho
                                       on j.CurrentHomeOwnerId = ho.HomeOwnerId
                                     left join (select COUNT(*) as NumberOfLineItems, ServiceCallId FROM ServiceCallLineItems group by ServiceCallId) li
                                       on wc.ServiceCallId = li.ServiceCallId
                                     inner join Employees e
                                       on wc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                     INNER JOIN Communities cm
                                       ON j.CommunityId = cm.CommunityId
                                     INNER JOIN Cities ci
                                       ON cm.CityId = ci.CityId
                                     {0} /* WHERE */
                                     {1} /* ORDER BY */";

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetOpenServiceCalls(IUser user)
        {
            var markets = user.Markets;

            var sql = string.Format(SqlTemplate, "WHERE ServiceCallStatusId<>@0 AND CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ")", "ORDER BY EmployeeName, wc.CreatedDate");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql, ServiceCallStatus.Complete.Value);
            return result;
        }

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetMyServiceCalls(IUser user)
        {
            var sql = string.Format(SqlTemplate, "WHERE ServiceCallStatusId<>@1 and EmployeeNumber=@0", "ORDER BY (7-DATEDIFF(d, wc.CreatedDate, GETDATE())), wc.CreatedDate, NumberOfLineItems DESC");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql, user.EmployeeNumber, ServiceCallStatus.Complete.Value);
            return result;
        }

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetSpecialProjects(IUser user)
        {
            var markets = user.Markets;

            var sql = string.Format(SqlTemplate, "WHERE ServiceCallStatusId<>@0 AND (CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ")) AND SpecialProject = 1", "ORDER BY EmployeeName, wc.CreatedDate");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql, ServiceCallStatus.Complete.Value);
            return result;
        }

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetEscalatedServiceCalls(IUser user)
        {
            var markets = user.Markets;

            var sql = string.Format(SqlTemplate, "WHERE ServiceCallStatusId<>@0 AND CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ") AND Escalated = 1", "ORDER BY EmployeeName, wc.CreatedDate");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql, ServiceCallStatus.Complete.Value);
            return result;
        }
    }
}
