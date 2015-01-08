namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using NPoco;

    public class ServiceCallCompleteWsrNotificationQueryHandler : IQueryHandler<ServiceCallCompleteWsrNotificationQuery, ServiceCallCompleteWsrNotificationModel>
    {
        private readonly IDatabase _database;

        public ServiceCallCompleteWsrNotificationQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public ServiceCallCompleteWsrNotificationModel Handle(ServiceCallCompleteWsrNotificationQuery query)
        {
            using (_database)
            {
                const string sql = @"SELECT ServiceCallId
                                    , ServiceCallNumber
                                    , WarrantyRepresentativeEmployeeId
                                    , ho.HomeOwnerName
                                    , ho.HomePhone
                                    , c.CommunityName
                                    , j.AddressLine
                                    , e.EmployeeNumber
                                  FROM ServiceCalls sc
                                  INNER JOIN Jobs j
                                    ON sc.JobId = j.JobId
                                  INNER JOIN communities c
                                    ON c.CommunityId = j.CommunityId
                                  INNER JOIN HomeOwners ho
                                    ON j.CurrentHomeOwnerId = ho.HomeOwnerId
                                  LEFT OUTER JOIN Employees e
                                    ON e.EmployeeId = sc.WarrantyRepresentativeEmployeeId
                                  WHERE sc.ServiceCallId = @0";

                var model = _database.SingleOrDefault<ServiceCallCompleteWsrNotificationModel>(sql, query.ServiceCallId);
                model.WarrantyRepresentativeEmployeeEmail = GetEmployeeEmail(model.EmployeeNumber);

                const string sqlcomments = @"SELECT ServiceCallNote
                                             FROM ServiceCallNotes
                                             WHERE ServiceCallId = @0";

                model.Comments = _database.Fetch<string>(sqlcomments, query.ServiceCallId);

                return model;
            }
        }

        public string GetEmployeeEmail(string employeeNumber)
        {
            if (employeeNumber == null)
                return null;

            var queryEmail = new Common.Security.Queries.GetEmailByEmployeeNumberQuery();
            return queryEmail.Execute(employeeNumber);
        }
    }
}