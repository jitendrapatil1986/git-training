using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.CreateServiceCall
{
    using NPoco;
    using Security;

    public class CreateServiceCallQueryHandler : IQueryHandler<CreateServiceCallQuery, CreateServiceCallModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public CreateServiceCallQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public CreateServiceCallModel Handle(CreateServiceCallQuery query)
        {
            using (_database)
            {
                return new CreateServiceCallModel
                    {
                        ServiceCallDetails = GetServiceCallDetails(query.JobId),
                    };
            }
        }

        private CreateServiceCallModel.ServiceCallDetail GetServiceCallDetails(Guid jobId)
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
                                WHERE j.JobId = @0
                                ORDER BY ho.HomeOwnerName, j.JobNumber";

            var result = _database.Single<CreateServiceCallModel.ServiceCallDetail>(sql, jobId);

            return result;
        }
    }


}
