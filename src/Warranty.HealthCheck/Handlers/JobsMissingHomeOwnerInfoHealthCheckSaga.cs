using System;
using System.Linq;
using System.Text;
using Common.Messages;
using log4net;
using MediatR;
using NServiceBus;
using NServiceBus.Saga;
using Warranty.HealthCheck.Mediatr;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Handlers
{
    public class JobsMissingHomeOwnerInfoHealthCheckSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        [Unique]
        public virtual DateTime? RunDate { get; set; }
        public virtual DateTime MaxCloseDate { get; set; }
    }

    public class JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner : IBusCommand
    {
        public DateTime MaxCloseDate { get; set; }

        public JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner()
        {
        }

        public JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner(DateTime maxCloseDate)
        {
            MaxCloseDate = maxCloseDate;
        }
    }

    public class JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner : IBusCommand
    {
        public DateTime MaxCloseDate { get; set; }

        public JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner()
        {
        }

        public JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner(DateTime maxCloseDate)
        {
            MaxCloseDate = maxCloseDate;
        }
    }

    public class JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables() { }

        public JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class CompareJobsFromTipsAndWarranty : IBusCommand
    {
    }

    public class InitiateJobsMissingHomeOwnerInfoHealthCheckSaga : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public InitiateJobsMissingHomeOwnerInfoHealthCheckSaga() { }

        public InitiateJobsMissingHomeOwnerInfoHealthCheckSaga(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class JobsMissingHomeOwnerInfoHealthCheckSaga : Saga<JobsMissingHomeOwnerInfoHealthCheckSagaData>,
        IAmStartedByMessages<InitiateJobsMissingHomeOwnerInfoHealthCheckSaga>,
        IHandleMessages<JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables>,
        IHandleMessages<JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner>,
        IHandleMessages<JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner>,
        IHandleMessages<CompareJobsFromTipsAndWarranty>
    {
        private readonly ILog _log;
        private readonly IMediator _mediator;

        public JobsMissingHomeOwnerInfoHealthCheckSaga()
        {
        }

        public JobsMissingHomeOwnerInfoHealthCheckSaga(ILog log, IMediator mediator)
        {
            _log = log;
            _mediator = mediator;
        }

        public void Handle(InitiateJobsMissingHomeOwnerInfoHealthCheckSaga message)
        {
            if (Data.RunDate.HasValue)
            {
                _log.WarnFormat("Trying to start a new instance of the JobsMissingHomeOwnerInfoHealthCheckSaga but it looks like one is already running for {0:d}.  If it's stuck, you can delete the SagaData record and try again.", message.RunDate);
                return;
            }

            _log.InfoFormat("Started new instance of the JobsMissingHomeOwnerInfoHealthCheckSaga for {0:d}", Data.RunDate);
            Data.RunDate = message.RunDate;
            Data.MaxCloseDate = Data.RunDate.Value.AddDays(-365);
            Bus.SendLocal(new JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables(Data.RunDate.Value));
        }

        public void Handle(JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables message)
        {
            _log.Info("Clearing any existing data in dbo.HEALTH_MissingJobs from previous checks");
            _mediator.Send(new ClearHomeOwnerMissingInfoTablesRequest());

            Bus.SendLocal(new JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner());
        }

        public void Handle(JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner message)
        {
            _log.InfoFormat("Loading all closed jobs from TIPS, with a homeowner, newer than {0:d}", Data.RunDate);
            _mediator.Send(new LoadClosedJobsFromTipsWithHomeOwnerRequest(Data.MaxCloseDate));

            Bus.SendLocal(new JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner(Data.RunDate.Value));
        }

        public void Handle(JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner message)
        {
            _log.InfoFormat("Loading all closed jobs from Warranty, without a homeowner, newer than {0:d}", Data.RunDate);
            _mediator.Send(new LoadClosedJobsFromWarrantyWithoutHomeOwnerRequest(Data.MaxCloseDate));

            Bus.SendLocal(new CompareJobsFromTipsAndWarranty());
        }

        public void Handle(CompareJobsFromTipsAndWarranty message)
        {
            var tipsJobsWithHomeOwner = _mediator.Send(new GetClosedJobsRequest(Systems.TIPS));
            var warrantyJobsWithNoHomeOwner = _mediator.Send(new GetClosedJobsRequest(Systems.Warranty));

            var jobsWithHomeOwnerInTipsButNotWarranty = tipsJobsWithHomeOwner
                .Where(tipsJob => warrantyJobsWithNoHomeOwner.Any(warrantyJob => warrantyJob == tipsJob))
                .ToList();

            if (!jobsWithHomeOwnerInTipsButNotWarranty.Any())
            {
                _log.Info("Could not find any jobs where a home owner existed in TIPS but not Warranty.");
                MarkAsComplete();
                return;
            }

            var notification = new StringBuilder();
            notification.AppendLine("Found the following closed jobs where there's a home owner in TIPS but is missing in Warranty:<br>");
            notification.AppendLine("<hr>");

            foreach (var job in jobsWithHomeOwnerInTipsButNotWarranty)
            {
                notification.AppendFormat("{0}<br>\n", job);
            }

            Bus.SendLocal(new Notification
            {
                Subject = string.Format("HEALTH CHECK FAILURE - {0} jobs with a home owner in TIPS but not Warranty", jobsWithHomeOwnerInTipsButNotWarranty.Count),
                Body = notification.ToString()
            });

            MarkAsComplete();
        }
    }    
}