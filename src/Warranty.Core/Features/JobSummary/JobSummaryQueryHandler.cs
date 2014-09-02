﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.JobSummary
{
    using NPoco;
    using Security;

    public class JobSummaryQueryHandler : IQueryHandler<JobSummaryQuery, JobSummaryModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public JobSummaryQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public JobSummaryModel Handle(JobSummaryQuery query)
        {
            var model = GetJobSummary(query.JobId);
            model.JobServiceCalls = GetJobServiceCalls(query.JobId);
            model.JobSelections = GetJobSelections(query.JobId);
            //model.JobPayments = GetJobPayments(query.JobId);

            return model;
        }

        private IEnumerable<JobSummaryModel.JobSelection> GetJobSelections(Guid jobId)
        {
            const string sql = @"SELECT [JobOptionId]
                                ,[JobId]
                                ,[OptionNumber]
                                ,[OptionDescription]
                                ,[Quantity]
                                FROM [JobOptions]
                                WHERE JobId = @0
                                ORDER BY OptionNumber";

            var result = _database.Fetch<JobSummaryModel.JobSelection>(sql, jobId);

            return result;
        }

        private JobSummaryModel GetJobSummary(Guid jobId)
        {
            const string sql = @"SELECT j.[JobId]
                                ,j.[JobNumber]
                                ,j.[CloseDate]
                                ,j.[AddressLine]
                                ,j.[City]
                                ,j.[StateCode]
                                ,j.[PostalCode]
                                ,j.[LegalDescription]
                                ,j.[CommunityId]
                                ,j.[CurrentHomeOwnerId]
                                ,j.[PlanType]
                                ,j.[PlanTypeDescription]
                                ,j.[PlanName]
                                ,j.[PlanNumber]
                                ,j.[Elevation]
                                ,j.[Swing]
                                ,j.[BuilderEmployeeId]
                                ,j.[SalesConsultantEmployeeId]
                                ,j.[WarrantyExpirationDate]
                                ,j.[DoNotContact]
                                ,j.[CreatedDate]
                                ,j.[CreatedBy]
                                ,j.[UpdatedDate]
                                ,j.[UpdatedBy]
                                ,j.[JdeIdentifier]
                                ,ho.HomeOwnerName
                                ,ho.HomePhone as PhoneNumber
                                ,ho.OtherPhone as OtherNumber
                                ,ho.WorkPhone1 as WorkNumber
                                ,ho.EmailAddress
                                ,be.EmployeeId as BuilderEmployeeId
                                ,be.EmployeeName as BuilderName
                                ,se.EmployeeId as SalesConsultantEmployeeId
                                ,se.EmployeeName as SalesConsultantName
                            FROM Jobs j
                            INNER JOIN HomeOwners ho
                            ON j.JobId = ho.JobId
                            LEFT JOIN Employees be
                            ON j.BuilderEmployeeId = be.EmployeeId
                            LEFT JOIN Employees se
                            ON j.SalesConsultantEmployeeId = se.EmployeeId
                            WHERE j.JobId = @0";

            var result = _database.Single<JobSummaryModel>(sql, jobId);
            
            return result;
        }

        private IEnumerable<JobSummaryModel.JobServiceCall> GetJobServiceCalls(Guid jobId)
        {
            const string sql = @"SELECT 
                                    wc.ServiceCallId as ServiceCallId
                                    ,Servicecallnumber as CallNumber
                                    ,wc.CreatedDate
                                    ,wc.CompletionDate
                                    ,case when (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) < 0 then 0 else (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) end as NumberOfDaysRemaining
                                    ,NumberOfLineItems
                                    ,LOWER(e.EmployeeName) as AssignedTo
                                    ,e.EmployeeNumber as AssignedToEmployeeNumber
                                    ,wc.SpecialProject as IsSpecialProject
                                    ,wc.Escalated as IsEscalated
                                    ,DATEDIFF(dd, wc.CreatedDate, wc.CompletionDate) as DaysOpenedFor
                                    ,DATEDIFF(yy, j.CloseDate, wc.CreatedDate) as YearsWithinWarranty
                                    ,j.CloseDate as WarrantyStartDate
                                    ,wc.EscalationReason
                                    ,wc.EscalationDate
                                    ,cc.ServiceCallCommentId
                                    ,cc.ServiceCallId
                                    ,cc.ServiceCallComment as Comment
                                FROM [ServiceCalls] wc
                                INNER JOIN Jobs j
                                ON wc.JobId = j.JobId
                                INNER JOIN HomeOwners ho
                                ON j.CurrentHomeOwnerId = ho.HomeOwnerId
                                INNER JOIN (select COUNT(*) as NumberOfLineItems, ServiceCallId FROM ServiceCallLineItems group by ServiceCallId) li
                                ON wc.ServiceCallId = li.ServiceCallId
                                INNER JOIN Employees e
                                ON wc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                LEFT JOIN ServiceCallComments cc
                                ON wc.ServiceCallId = cc.ServiceCallId
                                WHERE j.JobId = @0";

            var result = _database.FetchOneToMany<JobSummaryModel.JobServiceCall, JobSummaryModel.JobServiceCall.JobServiceCallComment>(x => x.ServiceCallId, sql, jobId);
            
            return result;
        }
    }
}
