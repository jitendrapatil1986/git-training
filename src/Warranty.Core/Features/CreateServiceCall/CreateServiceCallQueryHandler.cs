using System;
using System.Collections.Generic;

namespace Warranty.Core.Features.CreateServiceCall
{
    using System.Web.Mvc;
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
                var model = GetServiceCallDetails(query.JobId);
                model.ServiceCallTypeList = GetServiceCallTypeList();

                return model;
            }
        }

        private IEnumerable<SelectListItem> GetServiceCallTypeList()
        {
            const string sql = @"SELECT  ServiceCallTypeId as Value
                                        ,ServiceCallType as Text
                                FROM lookups.ServiceCallTypes
                                ORDER BY ServiceCallType";

            var result = _database.Fetch<SelectListItem>(sql);

            return result;
        }

        private CreateServiceCallModel GetServiceCallDetails(Guid jobId)
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
                                ORDER BY ho.HomeOwnerName, j.JobNumber";

            var result = _database.Single<CreateServiceCallModel>(sql, jobId);

            return result;
        }
    }


}
