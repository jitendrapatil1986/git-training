using System;
using Warranty.Core.Enumerations;
using System.Collections.Generic;
using NPoco;

namespace Warranty.Core.Features.RepServiceCalls
{
    public class ServiceRepServiceCallsQueryHandler : IQueryHandler<ServiceRepServiceCallsQuery, ServiceRepServiceCallsModel>
    {
        private readonly IDatabase _database;

        public ServiceRepServiceCallsQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public ServiceRepServiceCallsModel Handle(ServiceRepServiceCallsQuery query)
        {
            using (_database)
            {
                return new ServiceRepServiceCallsModel
                           {
                               EmployeeName = GetEmployeeName(query.EmployeeId),
                               OpenServiceCalls = GetServiceRepServiceCalls(ServiceCallStatus.Open, query.EmployeeId),
                               ClosedServiceCalls = GetServiceRepServiceCalls(ServiceCallStatus.Closed, query.EmployeeId),
                           };
            }
        }

        const string SqlTemplate = @"SELECT 
                                          wc.ServiceCallId as ServiceCallId
                                        , Servicecallnumber as CallNumber
                                        , j.AddressLine as [Address]
                                        , wc.CreatedDate 
                                        , wc.CompletionDate
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


        private string GetEmployeeName(Guid employeeId)
        {
            var sql = string.Format("Select EmployeeName from Employees where EmployeeId ='{0}'", employeeId);

            var result = _database.ExecuteScalar<string>(sql);
            return result;
        }

        private IEnumerable<ServiceRepServiceCallsModel.ServiceCall> GetServiceRepServiceCalls(ServiceCallStatus serviceCallStatus, Guid employeeId)
        {
            var whereClause = string.Format("WHERE wc.ServiceCallStatusId = {0} and wc.WarrantyRepresentativeEmployeeId = '{1}'",
                                               serviceCallStatus.Value, employeeId);

            var sql = string.Format(SqlTemplate, whereClause, "ORDER BY e.EmployeeName, wc.CreatedDate");

            var result = _database.Fetch<ServiceRepServiceCallsModel.ServiceCall>(sql);
            return result;
        }
    }
}
