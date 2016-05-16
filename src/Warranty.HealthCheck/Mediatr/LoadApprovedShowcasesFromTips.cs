using MediatR;
using Warranty.HealthCheck.Data;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Mediatr
{
    public class LoadApprovedShowcasesFromTipsHandler : RequestHandler<LoadApprovedShowcasesFromTips>
    {
        private readonly ITipsDatabase _tipsDatabase;
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public LoadApprovedShowcasesFromTipsHandler(ITipsDatabase tipsDatabase, IHealthCheckDatabase healthCheckDatabase)
        {
            _tipsDatabase = tipsDatabase;
            _healthCheckDatabase = healthCheckDatabase;
        }

        protected override void HandleCore(LoadApprovedShowcasesFromTips message)
        {
            var showcases = _tipsDatabase.Fetch<HEALTH_Showcase>(@"
                                            SELECT aj.JobNumber, @0 As System
                                            FROM SALE_ApprovalJob aj 
	                                            INNER JOIN SALE_Opportunity so ON so.SaleID = aj.SaleID
	                                            INNER JOIN JOB_Jobs j ON aj.JobNumber = j.JobNumber
	                                            INNER JOIN CRM_Opportunity co ON co.OpportunityID = so.OpportunityID
                                            WHERE 
	                                            j.JobType = 'S'
	                                            AND aj.Active = 1
	                                            AND aj.CancelFlag = 0
	                                            AND aj.Approved = 1", Systems.TIPS);

            _healthCheckDatabase.InsertBulk(showcases);
        }
    }

    public class LoadApprovedShowcasesFromTips : IRequest { }
}