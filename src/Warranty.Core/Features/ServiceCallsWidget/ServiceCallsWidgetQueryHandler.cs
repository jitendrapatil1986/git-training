namespace Warranty.Core.Features.ServiceCallsWidget
{
    using NPoco;
    using Security;

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
                const string sql = @"SELECT 
  wc.WarrantyCallId as ServiceCallId
, warrantycallnumber as CallNumber
, j.AddressLine as [Address]
, wc.CreatedDate 
, ho.HomeOwnerName
, case when (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) < 0 then 0 else (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) end as NumberOfDaysRemaining
, NumberOfLineItems
, ho.HomePhone as PhoneNumber
  FROM [WarrantyCalls] wc
  inner join Jobs j
  on wc.JobId = j.JobId
  inner join HomeOwners ho
  on j.CurrentHomeOwnerId = ho.HomeOwnerId
  inner join (select COUNT(*) as NumberOfLineItems, WarrantyCallId FROM WarrantyCallLineItems group by WarrantyCallId) li
  on wc.WarrantyCallId = li.WarrantyCallId
  inner join Employees e
  on wc.WarrantyRepresentativeEmployeeId = e.EmployeeId
  WHERE CompletionDate is null and EmployeeNumber=@0
  ORDER BY (7-DATEDIFF(d, wc.CreatedDate, GETDATE())), wc.CreatedDate, NumberOfLineItems DESC";

                var result = _database.Query<ServiceCallsWidgetModel.ServiceCall>(sql, user.EmployeeNumber);

                return new ServiceCallsWidgetModel
                           {
                               MyServiceCalls = result,
                           };
            }
        }
    }
}
