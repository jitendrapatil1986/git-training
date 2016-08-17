using System;
using Accounting.Events.Job;
using AutoMapper;
using Common.Messages;
using log4net;
using NServiceBus;
using NServiceBus.Saga;
using TIPS.Commands.Requests;
using TIPS.Commands.Responses;
using Warranty.Core;
using Warranty.Core.Entities;
using Warranty.Core.Features.Homeowner;
using Warranty.Core.Features.Job;
using Warranty.Core.Features.Vendor;

namespace Warranty.Server.Sagas
{
    public class JobClosedSaga  : Saga<JobClosedSagaData>,
                                IAmStartedByMessages<JobClosed>, 
                                IHandleMessages<HomeBuyerDetailsResponse>, 
                                IHandleMessages<JobClosedSaga_CloseJob>
    {
        private readonly ILog _log;
        private readonly IMediator _mediator;
        public JobClosedSaga() { }

        public JobClosedSaga(ILog log, IMediator mediator)
        {
            _log = log;
            _mediator = mediator;
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<JobClosedSaga_CloseJob>(m => m.JobNumber).ToSaga(s => s.JobNumber);
        }

        public void Handle(JobClosed message)
        {
            _log.InfoFormat("Received JobClosed for Job Number {0}", message.Job);

            var job = _mediator.Request(new GetJobQuery(message.Job));

            if (job == null)
            {
                _log.ErrorFormat("Job not found matching job number {0}", message.Job);
                return;
            }

            Data.JobNumber = message.Job;
            Data.JobId = job.JobId;
            Data.CloseDate = message.CloseDate;

            if (job.CurrentHomeOwnerId.HasValue == false)
            {
                _log.WarnFormat("Found Job {0} but it does not have a CurrentHomeOwner.  Requesting it from TIPS", message.Job);
                Bus.Send(new RequestHomeBuyerDetails(message.Job));
                return;
            }

            Data.CurrentHomeownerId = job.CurrentHomeOwnerId.Value;

            var homeOwner = _mediator.Request(new GetHomeOwnerQuery(job.JobNumber));
            if (homeOwner == null)
            {
                _log.WarnFormat("Job {0} has a CurrentHomeOwner but no HomeOwner could be found matching this job.  Requesting it from TIPS to make sure information is current", message.Job);
                Bus.Send(new RequestHomeBuyerDetails(message.Job));
                return;
            }

            _log.InfoFormat("HomeOwner data for Job {0} looks good, proceeding to close", message.Job);
            Bus.SendLocal(new JobClosedSaga_CloseJob(message.Job));
        }

        public void Handle(HomeBuyerDetailsResponse message)
        {
            _log.InfoFormat("Received HomeBuyerDetailsResponse requested for Job {0}", Data.JobNumber);

            var job = _mediator.Request(new GetJobQuery(Data.JobNumber));
            var homeOwner = _mediator.Request(new GetHomeOwnerQuery(Data.JobNumber));

            if (homeOwner == null)
            {
                _log.InfoFormat("Creating new HomeOwner for Job {0}", Data.JobNumber);
                var createCommand = Mapper.Map<CreateNewHomeOwnerCommand>(message);
                createCommand.JobId = Data.JobId;

                homeOwner = _mediator.Send(createCommand);
            }

            _log.InfoFormat("Assigning HomeOwner {0} to Job {1}", homeOwner.HomeOwnerId, Data.JobNumber);
            _mediator.Send(new AssignHomeOwnerToJobCommand(homeOwner.HomeOwnerId, job.JobId));

            _log.InfoFormat("HomeOwner information updated on Job {0}, proceeding to close", Data.JobNumber);
            Bus.SendLocal(new JobClosedSaga_CloseJob(Data.JobNumber));
        }

        public void Handle(JobClosedSaga_CloseJob message)
        {
            _log.InfoFormat("Closing Job {0}", Data.JobNumber);
            _mediator.Send(new CloseJobCommand(Data.JobId, Data.CloseDate));

            _log.InfoFormat("Updating vendor data for Job {0}", Data.JobNumber);
            _mediator.Send(new UpdateVendorDataCommand(Data.JobId));

            _log.InfoFormat("JobClosedSaga completed for Job {0}", Data.JobNumber);
            MarkAsComplete();
        }
    }

    public class JobClosedSaga_CloseJob : IBusCommand
    {
        public virtual string JobNumber { get; set; }

        public JobClosedSaga_CloseJob() { }
        public JobClosedSaga_CloseJob(string jobNumber)
        {
            JobNumber = jobNumber;
        }
    }

    public class JobClosedSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        public virtual Guid JobId { get; set; }
        public virtual Guid CurrentHomeownerId { get; set; }
        public virtual string JobNumber { get; set; }
        public virtual DateTime CloseDate { get; set; }
    }
}