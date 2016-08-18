using System;
using System.Linq;
using Common.Messages;
using log4net;
using MediatR;
using NServiceBus;
using NServiceBus.Saga;
using Warranty.HealthCheck.Mediatr;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Handlers
{
    public class JobsMissingHomeOwnerInfoHealthCheckSaga : Saga<JobsMissingHomeOwnerInfoHealthCheckSagaData>,
        IAmStartedByMessages<InitiateJobsMissingHomeOwnerInfoHealthCheckSaga>,
        IHandleMessages<JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables>,
        IHandleMessages<JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner>,
        IHandleMessages<JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner>,
        IHandleMessages<FindJobsMissingHomeOwnersInWarranty>,
        IHandleMessages<FindJobsWithNullCurrentHomeOwnerId>
    {
        private readonly ILog _log;
        private readonly IMediator _mediator;

        public JobsMissingHomeOwnerInfoHealthCheckSaga() { }

        public JobsMissingHomeOwnerInfoHealthCheckSaga(ILog log, IMediator mediator)
        {
            _log = log;
            _mediator = mediator;
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<InitiateJobsMissingHomeOwnerInfoHealthCheckSaga>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<FindJobsMissingHomeOwnersInWarranty>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<FindJobsWithNullCurrentHomeOwnerId>(m => m.RunDate).ToSaga(s => s.RunDate);
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
            Bus.SendLocal(new JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables(Data.RunDate.Value));
        }

        public void Handle(JobsMissingHomeOwnerInfoHealthCheckSagaCleanupTempTables message)
        {
            _log.Info("Clearing any existing data in dbo.HEALTH_JobsMissingHomeOwnerInfo from previous checks");
            _mediator.Send(new ClearHomeOwnerMissingInfoTablesRequest());

            Bus.SendLocal(new JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner(Data.RunDate.Value));
        }

        public void Handle(JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner message)
        {
            _log.InfoFormat("Loading all closed jobs from TIPS, with a homeowner, newer than {0:d}", Data.RunDate);
            _mediator.Send(new LoadClosedJobsFromTipsWithHomeOwnerRequest(Data.RunDate.Value));

            Bus.SendLocal(new JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner(Data.RunDate.Value));
        }

        public void Handle(JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner message)
        {
            _log.InfoFormat("Loading all closed jobs from Warranty, without a homeowner, newer than {0:d}", Data.RunDate);
            _mediator.Send(new LoadClosedJobsFromWarrantyWithoutHomeOwnerRequest(Data.RunDate.Value));

            Bus.SendLocal(new FindJobsMissingHomeOwnersInWarranty(Data.RunDate.Value));
        }

        public void Handle(FindJobsMissingHomeOwnersInWarranty message)
        {
            var tipsJobsWithHomeOwner = _mediator.Send(new GetClosedJobsRequest(Systems.TIPS));
            var warrantyJobsWithNoHomeOwner = _mediator.Send(new GetClosedJobsRequest(Systems.Warranty));

            var jobsWithHomeOwnerInTipsButNotWarranty = tipsJobsWithHomeOwner.Intersect(warrantyJobsWithNoHomeOwner).ToList();

            if (!jobsWithHomeOwnerInTipsButNotWarranty.Any())
            {
                _log.Info("Could not find any jobs where a home owner existed in TIPS but not Warranty.");
                Bus.SendLocal(new FindJobsWithNullCurrentHomeOwnerId(message.RunDate));
                return;
            }

            const string notificationMessage = @"Found the following closed jobs where there's a home owner in TIPS but not in Warranty";
            var subject = string.Format(@"{0} jobs with a home owner in TIPS but not Warranty", jobsWithHomeOwnerInTipsButNotWarranty.Count);

            var notification = Notification.Create(notificationMessage, subject, jobsWithHomeOwnerInTipsButNotWarranty);

            Bus.SendLocal(notification);
            Bus.SendLocal(new FindJobsWithNullCurrentHomeOwnerId(message.RunDate));
        }

        public void Handle(FindJobsWithNullCurrentHomeOwnerId message)
        {
            var jobsWithHomeOwnerButNullCurrentHomeOwnerId = _mediator.Send(new GetJobsWithHomeOwnerButNullCurrentHomeOwnerIdRequest(message.RunDate));

            if (!jobsWithHomeOwnerButNullCurrentHomeOwnerId.Any())
            {
                _log.Info("Could not find any jobs where a home owner exits but the CurrentHomeOwnerID is null in Warranty.");
                MarkAsComplete();
                return;
            }

            const string notificationMessage = @"Found the following jobs with a home owner but a null CurrentHomeOwnerID in Warranty";
            var subject = string.Format(@"{0} jobs with a home owner in TIPS but not Warranty", jobsWithHomeOwnerButNullCurrentHomeOwnerId.Count());

            var notification = Notification.Create(notificationMessage, subject, jobsWithHomeOwnerButNullCurrentHomeOwnerId);

            Bus.SendLocal(notification);
            MarkAsComplete();
        }
    }

    public class JobsMissingHomeOwnerInfoHealthCheckSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        [Unique]
        public virtual DateTime? RunDate { get; set; }
    }

    public class JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner() { }

        public JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedTipsJobsWithHomeOwner(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner() { }

        public JobsMissingHomeOwnerInfoHealthCheckSagaLoadClosedWarrantyJobsWithNoHomeOwner(DateTime runDate)
        {
            RunDate = runDate;
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

    public class InitiateJobsMissingHomeOwnerInfoHealthCheckSaga : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public InitiateJobsMissingHomeOwnerInfoHealthCheckSaga() { }

        public InitiateJobsMissingHomeOwnerInfoHealthCheckSaga(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class FindJobsMissingHomeOwnersInWarranty : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public FindJobsMissingHomeOwnersInWarranty() { }

        public FindJobsMissingHomeOwnersInWarranty(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class FindJobsWithNullCurrentHomeOwnerId : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public FindJobsWithNullCurrentHomeOwnerId() { }

        public FindJobsWithNullCurrentHomeOwnerId(DateTime runDate)
        {
            RunDate = runDate;
        }
    }
}