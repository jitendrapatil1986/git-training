using System;
using System.Collections.Generic;

namespace Warranty.Core.Features.ServiceCallSummary
{
    using Enumerations;
    using NPoco;
    using Security;

    public class ServiceCallSummaryQueryHandler : IQueryHandler<ServiceCallSummaryQuery, ServiceCallSummaryModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public ServiceCallSummaryQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public ServiceCallSummaryModel Handle(ServiceCallSummaryQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                return new ServiceCallSummaryModel
                    {
                        ServiceCallSummary = GetServiceCallSummary(query.ServiceCallId),
                        ServiceCallLines = GetServiceCallLines(query.ServiceCallId),
                        ServicCallNotes = GetServiceCallNotes(query.ServiceCallId),
                        AddServiceCallLineItem = new ServiceCallSummaryModel.NewServiceCallLineItem(query.ServiceCallId, SharedQueries.ProblemCodes.GetProblemCodeList(_database)),
                        CanApprove = user.IsInRole(UserRoles.WarrantyServiceCoordinator) || user.IsInRole(UserRoles.WarrantyServiceManager),
                        CanReassign = user.IsInRole(UserRoles.WarrantyServiceCoordinator) || user.IsInRole(UserRoles.WarrantyServiceManager),
                        CanReopenLines = user.IsInRole(UserRoles.WarrantyServiceCoordinator) || user.IsInRole(UserRoles.WarrantyServiceManager),
                    };
            }
        }

        private ServiceCallSummaryModel.ServiceCall GetServiceCallSummary(Guid serviceCallId)
        {
            const string sql = @"SELECT 
                                    wc.ServiceCallId as ServiceCallId
                                    , Servicecallnumber as CallNumber
                                    , j.AddressLine as [Address]
                                    , j.JobId
                                    , j.JobNumber
                                    , wc.CreatedDate
                                    , wc.CreatedBy
                                    , wc.CompletionDate
                                    , wc.ServiceCallstatusId as ServiceCallStatus
                                    , ho.HomeOwnerName
                                    , ho.HomeOwnerNumber
                                    , case when (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) < 0 then 0 else (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) end as NumberOfDaysRemaining
                                    , NumberOfLineItems
                                    , ho.HomeownerId
                                    , ho.HomePhone
                                    , ho.OtherPhone
                                    , ho.EmailAddress
                                    , LOWER(e.EmployeeName) as AssignedTo
                                    , e.EmployeeNumber as AssignedToEmployeeNumber
                                    , wc.SpecialProject as IsSpecialProject
                                    , wc.Escalated as IsEscalated
                                    , DATEDIFF(dd, wc.CreatedDate, wc.CompletionDate) as DaysOpenedFor
                                    , DATEDIFF(yy, j.CloseDate, wc.CreatedDate) as YearsWithinWarranty
                                    , j.CloseDate as WarrantyStartDate
                                    , wc.EscalationReason
                                    , wc.EscalationDate
                                    , d.DivisionName
                                    , p.ProjectName
                                    , cm.CommunityName
                                    , wc.HomeownerVerificationSignature
                                    , wc.HomeownerVerificationSignatureDate
                                FROM [ServiceCalls] wc
                                INNER JOIN Jobs j
                                ON wc.JobId = j.JobId
                                INNER JOIN HomeOwners ho
                                ON j.CurrentHomeOwnerId = ho.HomeOwnerId
                                INNER JOIN (select COUNT(*) as NumberOfLineItems, ServiceCallId FROM ServiceCallLineItems group by ServiceCallId) li
                                ON wc.ServiceCallId = li.ServiceCallId
                                LEFT JOIN Employees e
                                ON wc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                INNER JOIN Communities cm
                                ON j.CommunityId = cm.CommunityId
                                INNER JOIN Cities ci
                                ON cm.CityId = ci.CityId
                                INNER JOIN Divisions d
                                ON cm.DivisionId = d.DivisionId
                                INNER JOIN Projects p
                                ON cm.ProjectId = p.Projectid
                                WHERE wc.ServiceCallId = @0";

            var result = _database.Single<ServiceCallSummaryModel.ServiceCall>(sql, serviceCallId.ToString());
            
            return result;
        }

        private IEnumerable<ServiceCallSummaryModel.ServiceCallLine> GetServiceCallLines(Guid serviceCallId)
        {
            const string sql = @"SELECT
                                    li.ServiceCallLineItemId,
                                    li.ServiceCallId,
                                    li.LineNumber,
                                    li.ProblemCode,
                                    li.ProblemDescription,
                                    li.CauseDescription,
                                    li.ClassificationNote,
                                    li.LineItemRoot,
                                    li.CreatedDate,
                                    li.ServiceCallLineItemStatusId as ServiceCallLineItemStatus
                                FROM ServiceCalls wc
                                INNER JOIN ServiceCallLineItems li
                                ON wc.ServiceCallId = li.ServiceCallId
                                WHERE wc.ServiceCallId = @0
                                ORDER BY li.LineNumber DESC";

            var result = _database.Fetch<ServiceCallSummaryModel.ServiceCallLine>(sql, serviceCallId.ToString());



            return result;
        }

        private IEnumerable<ServiceCallSummaryModel.ServiceCallNote> GetServiceCallNotes(Guid serviceCallId)
        {
            const string sql = @"SELECT [ServiceCallNoteId]
                                      ,[ServiceCallId]
                                      ,[ServiceCallNote] as Note
                                      ,[ServiceCallLineItemId]
                                      ,[CreatedDate]
                                      ,[CreatedBy]     
                                FROM [ServiceCallNotes]
                                WHERE ServiceCallId = @0";

            var result = _database.Fetch<ServiceCallSummaryModel.ServiceCallNote>(sql, serviceCallId.ToString());

            return result;
        }
    }
}
