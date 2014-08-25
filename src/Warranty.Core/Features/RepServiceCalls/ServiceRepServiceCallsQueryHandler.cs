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
                               OpenServiceCalls = GetServiceRepOpenServiceCalls(query.EmployeeId),
                               ClosedServiceCalls = GetServiceRepClosedServiceCalls(query.EmployeeId),
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
                                        , wc.SpecialProject as IsSpecialProject
                                        , wc.Escalated as IsEscalated
                                        , DATEDIFF(dd, wc.CreatedDate, wc.CompletionDate) as DaysOpenedFor
                                        , DATEDIFF(yy, j.CloseDate, wc.CreatedDate) as YearsWithinWarranty
                                        , j.CloseDate as WarrantyStartDate
                                        , wc.EscalationReason
                                        , wc.EscalationDate
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
            var result = _database.Single<string>("Select EmployeeName from Employees where EmployeeId = @0", employeeId);
            return result;
        }

        private IEnumerable<ServiceRepServiceCallsModel.ServiceCall> GetServiceRepOpenServiceCalls(Guid employeeId)
        {
            var whereClause = GetWhereClause();

            var sql = string.Format(SqlTemplate, whereClause, "ORDER BY NumberOfDaysRemaining, ho.HomeOwnerName");

            var result = _database.Fetch<ServiceRepServiceCallsModel.ServiceCall>(sql, ServiceCallStatus.Open.Value, employeeId);
            return result;
        }

        private IEnumerable<ServiceRepServiceCallsModel.ServiceCall> GetServiceRepClosedServiceCalls(Guid employeeId)
        {
            var whereClause = GetWhereClause();

            var sql = string.Format(SqlTemplate, whereClause, "ORDER BY wc.CompletionDate desc, ho.HomeOwnerName");

            var result = _database.Fetch<ServiceRepServiceCallsModel.ServiceCall>(sql, ServiceCallStatus.Closed.Value, employeeId);
            return result;
        }

        private static string GetWhereClause()
        {
            return "WHERE wc.ServiceCallStatusId = @0 and wc.WarrantyRepresentativeEmployeeId = @1 and wc.CreatedDate > DATEADD(year, -1, GETDATE())";
        }
    }
}
