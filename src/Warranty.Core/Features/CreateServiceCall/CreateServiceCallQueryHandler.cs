namespace Warranty.Core.Features.CreateServiceCall
{
    using System;
    using NPoco;

    public class CreateServiceCallQueryHandler : IQueryHandler<CreateServiceCallQuery, CreateServiceCallModel>
    {
        private readonly IDatabase _database;

        public CreateServiceCallQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public CreateServiceCallModel Handle(CreateServiceCallQuery query)
        {
            using (_database)
            {
                const string sql = @"SELECT TOP 1 ho.HomeOwnerId
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
                                        ,j.CloseDate as WarrantyStartDate
                                        ,DATEDIFF(yy, j.CloseDate, GETDATE()) as YearsWithinWarranty
                                        ,ISNULL(sales.EmployeeName, '') as SalesPersonName
                                        ,ISNULL(builder.EmployeeName, '') as BuilderName
                                FROM HomeOwners ho
                                INNER JOIN Jobs j
                                ON ho.JobId = j.JobId
                                LEFT JOIN Employees sales
                                ON j.SalesConsultantEmployeeId = sales.EmployeeId
                                LEFT JOIN Employees builder
                                ON j.BuilderEmployeeId = builder.EmployeeId
                                WHERE j.JobId = @0
                                ORDER BY ho.HomeownerNumber DESC";

                var result = _database.Single<CreateServiceCallModel>(sql, query.JobId);
                return result;
            }
        }
    }


}
