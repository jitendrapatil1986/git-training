using System.Collections.Generic;

namespace Warranty.Core.Features.CreateServiceCallCustomerSearch
{
    using NPoco;
    using Security;

    public class CreateServiceCallCustomerSearchQueryHandler : IQueryHandler<CreateServiceCallCustomerSearchQuery, IEnumerable<CustomerSearchModel>>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public CreateServiceCallCustomerSearchQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public IEnumerable<CustomerSearchModel> Handle(CreateServiceCallCustomerSearchQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                return GetCustomers(query.Query);
            }
        }

        private IEnumerable<CustomerSearchModel> GetCustomers(string searchCriteria)
        {
            const string sql = @"SELECT  ho.HomeOwnerId
                                        ,j.JobId
                                        ,ho.[HomeOwnerNumber]
                                        ,ho.[HomeOwnerName]
                                        ,ho.[HomePhone]
                                        ,ho.[OtherPhone]
                                        ,ho.[WorkPhone1]
                                        ,ho.[WorkPhone2]
                                        ,ho.[EmailAddress]
                                        ,ho.[CreatedDate]
                                        ,ho.[CreatedBy]
                                        ,j.AddressLine as [Address1]
                                        ,j.City
                                        ,j.StateCode
                                        ,j.PostalCode
                                        ,j.JobNumber
                                        , j.CloseDate as WarrantyStartDate
                                        , DATEDIFF(yy, j.CloseDate, GETDATE()) as YearsWithinWarranty
                                FROM HomeOwners ho
                                INNER JOIN Jobs j
                                ON ho.JobId = j.JobId
                                WHERE (ho.HomeOwnerName LIKE '%' + @0 + '%') OR 
                                (j.AddressLine LIKE '%' + @0 + '%') OR 
                                (j.JobNumber LIKE '%' + @0 + '%')
                                ORDER BY ho.HomeOwnerName, j.JobNumber";

            var result = _database.Fetch<CustomerSearchModel>(sql, searchCriteria);

            return result;
        }
    }
}
