using System;
using System.Collections.Generic;

namespace Warranty.Core.Features.JobSummary
{
    using System.Configuration;
    using System.Net.Http;
    using Enumerations;
    using JobServiceApiHelpers;
    using NPoco;
    using Security;
    using Services;
    using System.Linq;
    using Warranty.Core.Extensions;
    using HttpApiClient = JobService.Client.Api.HttpApiClient;

    public class JobSummaryQueryHandler : IQueryHandler<JobSummaryQuery, JobSummaryModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly IHomeownerAdditionalContactsService _homeownerAdditionalContactsService;

        public JobSummaryQueryHandler(IDatabase database, IUserSession userSession, IHomeownerAdditionalContactsService homeownerAdditionalContactsService)
        {
            _database = database;
            _userSession = userSession;
            _homeownerAdditionalContactsService = homeownerAdditionalContactsService;
        }

        public JobSummaryModel Handle(JobSummaryQuery query)
        {
            var model = GetJobSummary(query.JobId);
            model.JobServiceCalls = GetJobServiceCalls(query.JobId);
            model.JobSelections = GetJobSelections(query.JobId);
            model.JobNotes = GetJobNotes(query.JobId);
            model.Attachments = GetJobAttachments(query.JobId);
            model.Homeowners = GetJobHomeowners(query.JobId);
            model.JobPayments = GetJobPayments(query.JobId);
            model.AdditionalContacts = _homeownerAdditionalContactsService.Get(model.HomeownerId);
            model.Vendors = GetJobVendors(query.JobId);
            model.CostCodes = model.Vendors.SelectMany(cc => cc.CostCodes)
                                   .Distinct()
                                   .OrderBy(x => x.CostCodeDescription);

            return model;
        }

        private IEnumerable<JobSummaryModel.JobSelection> GetJobSelections(Guid jobId)
        {
            var apiServiceUrl = ConfigurationManager.AppSettings["ApiServiceUrl"];

            const string sql = @"SELECT [JobNumber] FROM Jobs                                
                                WHERE JobId = @0";

            var jobNumber = _database.ExecuteScalar<string>(sql, jobId);

            var client = new JobService.Client.Api.Endpoints.ArchivedBuildConfigurationEndpointProxy(apiServiceUrl, new HttpApiClient(), new ApiJsonConverter());
            try
            {
                var jobInfo = client.Get(jobNumber);
                return jobInfo.Selections.Select(x => new JobSummaryModel.JobSelection
                {
                    OptionNumber = x.OptionNumber,
                    OptionDescription = x.Description
                });
            }
            catch (HttpRequestException)
            {
                return new List<JobSummaryModel.JobSelection>();
            }
        }

        private JobSummaryModel GetJobSummary(Guid jobId)
        {
            const string sql = @"SELECT TOP 1 j.[JobId]
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
                                ,ho.HomeownerId
                                ,ho.HomeOwnerName
                                ,ho.HomeOwnerNumber
                                ,ho.HomePhone as HomePhone
                                ,ho.OtherPhone as OtherPhone
                                ,ho.WorkPhone1 as WorkNumber
                                ,ho.EmailAddress
                                ,be.EmployeeId as BuilderEmployeeId
                                ,be.EmployeeName as BuilderName
                                ,se.EmployeeId as SalesConsultantEmployeeId
                                ,se.EmployeeName as SalesConsultantName
                                , DATEDIFF(yy, j.CloseDate, getdate()) as YearsWithinWarranty
                                , j.CloseDate as WarrantyStartDate
                                , wsr.EmployeeName as ServiceRepName
                                , pr.ProjectName
                                , co.CommunityName
                            FROM Jobs j
                            INNER JOIN HomeOwners ho
                            ON j.JobId = ho.JobId
                            LEFT JOIN Employees be
                            ON j.BuilderEmployeeId = be.EmployeeId
                            LEFT JOIN Employees se
                            ON j.SalesConsultantEmployeeId = se.EmployeeId
                            LEFT OUTER JOIN CommunityAssignments ca
                            ON j.CommunityId = ca.CommunityId
                            LEFT OUTER JOIN Employees wsr
                            ON ca.EmployeeId = wsr.EmployeeId
                            LEFT OUTER JOIN Communities co
                            ON co.CommunityId = ca.CommunityId
                            LEFT OUTER JOIN Projects pr
                            ON pr.ProjectId = co.ProjectId
                            WHERE j.JobId = @0
                            ORDER BY ho.HomeownerNumber DESC";

            var result = _database.Single<JobSummaryModel>(sql, jobId);

            return result;
        }

        private IEnumerable<JobSummaryModel.JobServiceCall> GetJobServiceCalls(Guid jobId)
        {
            var user = _userSession.GetCurrentUser();
            const string sql = @"SELECT 
                                    wc.ServiceCallId as ServiceCallId
                                    ,wc.ServiceCallStatusId as ServiceCallStatus
                                    ,Servicecallnumber as CallNumber
                                    ,ISNULL(STUFF((SELECT '| ' + l.ProblemDescription
                                                    FROM ServiceCallLineItems l WHERE l.ServiceCallId = wc.servicecallid
                                                    FOR xml path('')),1,1,''), '') AS Summary
                                    ,wc.CreatedDate
                                    ,wc.CompletionDate
                                    ,case when (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) < 0 then 0 else (7-DATEDIFF(d, wc.CreatedDate, GETDATE())) end as NumberOfDaysRemaining
                                    ,ISNULL(NumberOfLineItems, 0) as NumberOfLineItems
                                    ,LOWER(e.EmployeeName) as AssignedTo
                                    ,e.EmployeeNumber as AssignedToEmployeeNumber
                                    ,wc.SpecialProject as IsSpecialProject
                                    ,wc.Escalated as IsEscalated
                                    ,DATEDIFF(dd, wc.CreatedDate, wc.CompletionDate) as DaysOpenedFor
                                    ,DATEDIFF(yy, j.CloseDate, wc.CreatedDate) as YearsWithinWarranty
                                    ,j.CloseDate as WarrantyStartDate
                                    ,wc.EscalationReason
                                    ,wc.EscalationDate
                                    ,cc.ServiceCallNoteId
                                    ,cc.ServiceCallId
                                    ,cc.ServiceCallNote as Note
                                FROM [ServiceCalls] wc
                                INNER JOIN Jobs j
                                ON wc.JobId = j.JobId
                                INNER JOIN HomeOwners ho
                                ON j.CurrentHomeOwnerId = ho.HomeOwnerId
                                LEFT JOIN (select COUNT(*) as NumberOfLineItems, ServiceCallId FROM ServiceCallLineItems group by ServiceCallId) li
                                ON wc.ServiceCallId = li.ServiceCallId
                                LEFT JOIN Employees e
                                ON wc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                LEFT JOIN ServiceCallNotes cc
                                ON wc.ServiceCallId = cc.ServiceCallId
                                WHERE j.JobId = @0";

            var result = _database.FetchOneToMany<JobSummaryModel.JobServiceCall, JobSummaryModel.JobServiceCall.JobServiceCallNote>(x => x.ServiceCallId, sql, jobId);

            result.ForEach(x =>
            {
                x.CanApprove = user.IsInRole(UserRoles.WarrantyServiceCoordinator) ||
                               user.IsInRole(UserRoles.CustomerCareManager);
            });

            return result;
        }

        private IEnumerable<JobSummaryModel.JobNote> GetJobNotes(Guid jobId)
        {
            const string sql = @"SELECT [JobNoteId]
                                  ,[JobId]
                                  ,[Note]
                                  ,[CreatedBy]
                                  ,[CreatedDate]
                                FROM [dbo].[JobNotes]
                                WHERE JobId = @0
                                ORDER BY CreatedDate DESC";

            var result = _database.Fetch<JobSummaryModel.JobNote>(sql, jobId);

            return result;
        }

        private IEnumerable<JobSummaryModel.Attachment> GetJobAttachments(Guid jobId)
        {
            const string sql = @"SELECT [JobAttachmentId]
                                  ,[JobId]
                                  ,[FilePath]
                                  ,[DisplayName]
                                  ,[IsDeleted]
                                  ,[CreatedBy]
                                  ,[CreatedDate]
                                FROM [dbo].[JobAttachments]
                                WHERE JobId = @0 AND IsDeleted=0
                                ORDER BY CreatedDate DESC";

            var result = _database.Fetch<JobSummaryModel.Attachment>(sql, jobId);

            return result;
        }

        private IEnumerable<JobSummaryModel.Homeowner> GetJobHomeowners(Guid jobId)
        {
            const string sql = @"SELECT h.[HomeownerName]
                                    ,h.[CreatedBy]
                                    ,h.[CreatedDate]
                                FROM Homeowners h
                                INNER JOIN [dbo].[Jobs] j
                                ON h.JobId = j.JobId
                                WHERE j.JobId = @0
                                AND h.HomeownerNumber > 1
                                ORDER BY h.CreatedDate DESC";

            var result = _database.Fetch<JobSummaryModel.Homeowner>(sql, jobId);

            return result;
        }

        private IEnumerable<JobSummaryModel.JobPayment> GetJobPayments(Guid jobId)
        {
            const string sql = @"SELECT sc.ServiceCallNumber,
                                    p.PaymentId
                                    , p.VendorName
                                    , p.Amount
                                    , p.PaymentStatus
                                    , p.InvoiceNumber
                                    , p.CreatedDate as PaymentCreatedDate
                                    , p.HoldComments
                                    , p.HoldDate
                                    , b.backchargeId
                                    , b.BackchargeVendorName
                                    , b.BackchargeReason
                                    , b.BackchargeAmount
                                    , b.PersonNotified
                                    , b.PersonNotifiedPhoneNumber
                                    , b.PersonNotifiedDate
                                    , b.BackchargeResponseFromVendor
                                    , b.BackchargeStatus
                                    , b.HoldComments backchargeHoldComments
                                    , b.HoldDate backchargeHoldDate
                                    , b.DenyComments backchargeDenyComments
                                    , b.DenyDate backchargeDenyDate
                                    , CASE WHEN b.BackchargeVendorNumber IS NOT NULL THEN 1 ELSE 0 END AS IsBackcharge
                                FROM payments p
                                LEFT JOIN backcharges b
                                ON p.PaymentId = b.PaymentId
                                INNER JOIN ServiceCallLineItems scli
                                ON scli.ServiceCallLineItemId = p.ServiceCallLineItemId
                                INNER JOIN ServiceCalls sc
                                ON scli.ServiceCallId = sc.ServiceCallId
                                INNER JOIN Jobs j
                                ON sc.JobId = j.JobId
                                WHERE j.JobId = @0 ORDER BY p.CreatedDate desc";

            var result = _database.Fetch<JobSummaryModel.JobPayment>(sql, jobId);

            return result;
        }

        private IEnumerable<JobSummaryModel.Vendor> GetJobVendors(Guid jobId)
        {
            const string emailIdentifier = "E-mail";
            const string sql = @"SELECT v.VendorId, v.Number, v.Name, ci.Value, ci.Type, jbcc.CostCode, jbcc.CostCodeDescription FROM Vendors v
                                INNER JOIN (SELECT VendorId, number as Value, Type as Type FROM VendorPhones WHERE number IS NOT NULL OR LTRIM(RTRIM(number)) <> ''
                                    UNION
                                    SELECT VendorId, email as Value, '{0}' as Type FROM VendorEmails  WHERE LTRIM(RTRIM(email)) <> '') as ci
                                    on v.vendorid = ci.vendorid
                                INNER JOIN JobVendorCostCodes jbcc
                                ON jbcc.VendorId = v.VendorId
                                AND jbcc.JobId = @0";

            var result = _database.Fetch<VendorDto>(string.Format(sql, emailIdentifier), jobId).ToList();

            var vendors =
            result.Select(x => new JobSummaryModel.Vendor
            {
                Name = x.Name,
                Number = x.Number,
                VendorId = x.VendorId,
                CostCodes =
                result.Where(v => v.VendorId == x.VendorId)
                .Select(cc => new JobSummaryModel.CostCodeModel
                {
                    CostCode = cc.CostCode,
                    CostCodeDescription = cc.CostCodeDescription
                }).Distinct().OrderBy(ob => ob.CostCode).ToList(),
                ContactInfo =
                result.Where(v => v.VendorId == x.VendorId)
                                              .Select(cc => new JobSummaryModel.Vendor.ContactInfoModel()
                                              {
                                                  Value = cc.Type != emailIdentifier ? cc.Value.ToPhoneNumberWithExtension():cc.Value,
                                                  Type = cc.Type
                                              }).Distinct().OrderBy(ob => ob.Value).ToList()
            }).Distinct().OrderBy(x => x.Name).ToList();

            vendors.ForEach(x =>
                {
                    x.CostCodesSeparatedByComma = x.CostCodes.Select(z => z.CostCodeDescription).OrderBy(y => y).CommaSeparate();
                });

            return vendors;
        }

        public class VendorDto
        {
            public Guid VendorId { get; set; }
            public string Name { get; set; }
            public string Number { get; set; }
            public string Value { get; set; }
            public string Type { get; set; }
            public string CostCode { get; set; }
            public string CostCodeDescription { get; set; }
        }


    }
}
