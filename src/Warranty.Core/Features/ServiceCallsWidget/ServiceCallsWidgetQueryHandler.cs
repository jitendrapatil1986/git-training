namespace Warranty.Core.Features.ServiceCallsWidget
{
    using System.Collections.Generic;
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
                               OverdueServiceCalls = GetOverdueServiceCalls(user),
                               SpecialProjectServiceCalls = GetSpecialProjects(user),
                           };
            }
        }
        
        const string SqlTemplate = @"SELECT 
                                          wc.ServiceCallId as ServiceCallId
                                        , Servicecallnumber as CallNumber
                                        , j.AddressLine as [Address]
                                        , wc.CreatedDate 
                                        , ho.HomeOwnerName
                                        , case when (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) < 0 then 0 else (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) end as NumberOfDaysRemaining
                                        , NumberOfLineItems
                                        , ho.HomePhone as PhoneNumber
                                        , e.EmployeeName as AssignedTo
                                        , e.EmployeeNumber as AssignedToEmployeeNumber
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

            var sql = string.Format(SqlTemplate, "WHERE CompletionDate is null AND DATEADD(dd, 7, wc.CreatedDate) <= getdate() AND CityCode IN ("+markets.CommaSeparateWrapWithSingleQuote()+")", "ORDER BY EmployeeName, wc.CreatedDate");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql);
            return result;
        }

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetMyServiceCalls(IUser user)
        {
            var sql = string.Format(SqlTemplate, "WHERE CompletionDate is null and EmployeeNumber=@0", "ORDER BY (7-DATEDIFF(d, wc.CreatedDate, GETDATE())), wc.CreatedDate, NumberOfLineItems DESC");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql, user.EmployeeNumber);
            return result;
        }

        private IEnumerable<ServiceCallsWidgetModel.ServiceCall> GetSpecialProjects(IUser user)
        {
            //TODO: Uncomment condition for 'IsSpecialProject' once column is added to the db.
            var markets = user.Markets;

            var sql = string.Format(SqlTemplate, "WHERE CompletionDate is null AND DATEADD(dd, 7, wc.CreatedDate) <= getdate() AND CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ") /*AND IsSpecialProject = 1*/", "ORDER BY EmployeeName, wc.CreatedDate");

            var result = _database.Fetch<ServiceCallsWidgetModel.ServiceCall>(sql);
            return result;
        }
    }
}
