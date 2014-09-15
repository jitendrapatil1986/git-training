using NPoco;

namespace Warranty.Core.Features.CreateServiceCall
{
    public class NewServiceCallAssignedToWsrNotificationQueryHandler : IQueryHandler<NewServiceCallAssignedToWsrNotificationQuery, NewServiceCallAssignedToWsrNotificationModel>
    {
        private readonly IDatabase _database;

        public NewServiceCallAssignedToWsrNotificationQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public NewServiceCallAssignedToWsrNotificationModel Handle(NewServiceCallAssignedToWsrNotificationQuery query)
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
                          INNER JOIN Employees e
                            ON e.EmployeeId = sc.WarrantyRepresentativeEmployeeId 
                          WHERE sc.ServiceCallId = @0";

                var model = _database.SingleOrDefault<NewServiceCallAssignedToWsrNotificationModel>(sql, query.ServiceCallId);
                model.WarrantyRepresentativeEmployeeEmail = GetEmployeeEmail(model.EmployeeNumber);


                const string sqlcomments = @"  SELECT ServiceCallComment 
                                                FROM ServiceCallComments 
                                                WHERE ServiceCallId = @0";

                model.Comments = _database.Fetch<string>(sqlcomments, query.ServiceCallId);
                return model;
            }
        }

        public string GetEmployeeEmail(string employeeNumber)
        {
            var queryEmail = new Common.Security.Queries.GetEmailByEmployeeNumberQuery();
            return queryEmail.Execute(employeeNumber);
        }
    }
}