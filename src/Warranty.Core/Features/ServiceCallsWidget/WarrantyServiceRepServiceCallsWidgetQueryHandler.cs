namespace Warranty.Core.Features.ServiceCallsWidget
{
    using System.Collections.Generic;
    using Enumerations;
    using Extensions;
    using NPoco;
    using Security;

    public class WarrantyServiceRepServiceCallsWidgetQueryHandler : IQueryHandler<WarrantyServiceRepServiceCallsWidgetQuery, ServiceCallsWidgetModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WarrantyServiceRepServiceCallsWidgetQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public ServiceCallsWidgetModel Handle(WarrantyServiceRepServiceCallsWidgetQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                return new ServiceCallsWidgetModel
                           {
                               MyServiceCalls = GetMyServiceCalls(user),
                               OverdueServiceCalls = GetOverdueServiceCalls(user),
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
                                        , NumberOfLineItems
                                        , ho.HomePhone as PhoneNumber
                                        , e.EmployeeName as AssignedTo
                                        , e.EmployeeNumber as AssignedToEmployeeNumber
                                        , wc.EscalationDate
                                        , wc.EscalationReason
                                        , DATEDIFF(yy, j.CloseDate, wc.CreatedDate) as YearsWithinWarranty
                                        , j.CloseDate as WarrantyStartDate
                                        , j.JobNumber
                                     FROM [ServiceCalls] wc
                                     inner join Jobs j
                                       on wc.JobId = j.JobId
                                     inner join HomeOwners ho
                                       on j.CurrentHomeOwnerId = ho.HomeOwnerId
                                     inner join (select COUNT(*) as NumberOfLineItems, ServiceCallId FROM ServiceCallLineItems group by ServiceCallId) li
                                       on wc.ServiceCallId = li.ServiceCallId
                                     inner join Employees e
                                       on wc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                     INNER JOIN Communities cm
                                       ON j.CommunityId = cm.CommunityId
                                     INNER JOIN Cities ci
                                       ON cm.CityId = ci.CityId
                                     {0} /* WHERE */
                                     {1} /* ORDER BY */";

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetOverdueServiceCalls(IUser user)
        {
            var markets = user.Markets;

            var sql = string.Format(SqlTemplate, "WHERE ServiceCallStatusId=@1 AND DATEADD(dd, 7, wc.CreatedDate) <= getdate() AND CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ") AND EmployeeNumber=@0", "ORDER BY EmployeeName, wc.CreatedDate");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql, user.EmployeeNumber, ServiceCallStatus.Complete.Value);
            return result;
        }

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetMyServiceCalls(IUser user)
        {
            var sql = string.Format(SqlTemplate, "WHERE ServiceCallStatusId=@1 and EmployeeNumber=@0", "ORDER BY (7-DATEDIFF(d, wc.CreatedDate, GETDATE())), wc.CreatedDate, NumberOfLineItems DESC");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql, user.EmployeeNumber, ServiceCallStatus.Complete.Value);
            return result;
        }

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetSpecialProjects(IUser user)
        {
            var markets = user.Markets;

            var sql = string.Format(SqlTemplate, "WHERE ServiceCallStatusId=@1 AND (CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ") OR EmployeeNumber=@0) AND SpecialProject = 1", "ORDER BY EmployeeName, wc.CreatedDate");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql, user.EmployeeNumber, ServiceCallStatus.Complete.Value);
            return result;
        }

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetEscalatedServiceCalls(IUser user)
        {
            var markets = user.Markets;

            var sql = string.Format(SqlTemplate, "WHERE ServiceCallStatusId=@1 AND EmployeeNumber=@0 AND CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ") AND Escalated = 1", "ORDER BY EmployeeName, wc.CreatedDate");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql, user.EmployeeNumber, ServiceCallStatus.Complete.Value);
            return result;
        }
    }
}