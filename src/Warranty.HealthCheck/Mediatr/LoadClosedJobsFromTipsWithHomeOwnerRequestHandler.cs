using System;
using MediatR;
using Warranty.HealthCheck.Data;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Mediatr
{
    public class LoadClosedJobsFromTipsWithHomeOwnerRequest : IRequest
    {
        public DateTime MaxCloseDate { get; set; }

        public LoadClosedJobsFromTipsWithHomeOwnerRequest()
        {
        }

        public LoadClosedJobsFromTipsWithHomeOwnerRequest(DateTime maxCloseDate)
        {
            MaxCloseDate = maxCloseDate;
        }
    }

    public class LoadClosedJobsFromTipsWithHomeOwnerRequestHandler : RequestHandler<LoadClosedJobsFromTipsWithHomeOwnerRequest>
    {
        private readonly ITipsDatabase _tipsDatabase;
        private readonly IHealthCheckDatabase _healhCheckDatabase;

        public LoadClosedJobsFromTipsWithHomeOwnerRequestHandler(ITipsDatabase tipsDatabase, IHealthCheckDatabase healhCheckDatabase)
        {
            _tipsDatabase = tipsDatabase;
            _healhCheckDatabase = healhCheckDatabase;
        }

        protected override void HandleCore(LoadClosedJobsFromTipsWithHomeOwnerRequest message)
        {
            var jobsWithoutAHomeOwnerInTips = _tipsDatabase.Fetch<HEALTH_ClosedJobs>(@"SELECT 
		                                                                                J.JobNumber
		                                                                                ,@1 AS System
	                                                                                FROM dbo.JOB_Jobs J
	                                                                                INNER JOIN dbo.Sale_ApprovalJob AJ
		                                                                                ON AJ.JobNumber = J.JobNumber
	                                                                                INNER JOIN dbo.Sale_Opportunity SO
		                                                                                ON SO.SaleId = AJ.SaleId
	                                                                                INNER JOIN dbo.CRM_Opportunity O
		                                                                                ON O.OpportunityId = SO.OpportunityId
	                                                                                INNER JOIN dbo.CRM_Contacts C
		                                                                                ON C.ContactID = O.ContactID
	                                                                                LEFT JOIN dbo.CRM_ContactEmail CE
		                                                                                ON CE.ContactID = C.ContactId
		                                                                                AND CE.PrimaryEmail = 1
	                                                                                LEFT JOIN dbo.CRM_ContactPhone CP
		                                                                                ON CP.ContactID = C.ContactID
		                                                                                AND CP.PrimaryPhone = 1
	                                                                                WHERE J.CloseDate IS NOT NULL
                                                                                        AND J.CloseDate > @0
		                                                                                AND AJ.Active = 1
		                                                                                AND AJ.CancelFlag = 0
		                                                                                AND SO.SaleDate IS NOT NULL;", message.MaxCloseDate, Systems.TIPS);

            _healhCheckDatabase.InsertBulk(jobsWithoutAHomeOwnerInTips);
        }
    }
}