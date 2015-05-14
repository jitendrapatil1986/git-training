using System;

namespace Warranty.Core.Features.CreateServiceCallVerifyCustomer
{
    using NPoco;
    using Common.Security.User.Session;

    public class CreateServiceCallVerifyCustomerQueryHandler : IQueryHandler<CreateServiceCallVerifyCustomerQuery, CreateServiceCallVerifyCustomerModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public CreateServiceCallVerifyCustomerQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;

        }

        public CreateServiceCallVerifyCustomerModel Handle(CreateServiceCallVerifyCustomerQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                var model = GetCustomerById(query.HomeOwnerId);

                return model;
            }
        }

        private CreateServiceCallVerifyCustomerModel GetCustomerById(Guid homeOwnerId)
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
                                WHERE ho.HomeOwnerid = @0
                                ORDER BY ho.HomeOwnerName, j.JobNumber";

            var result = _database.Single<CreateServiceCallVerifyCustomerModel>(sql, homeOwnerId);

            return result;
        }
    }
}
