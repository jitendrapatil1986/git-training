using System;
using NServiceBus;
using NServiceBus.Saga;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Server.Sagas
{
    public class BuyerTransferredToNewLotSaga : Saga<BuyerTransferredToNewLotSagaData>,
        IAmStartedByMessages<BuyerTransferredToNewLot>,
        IHandleMessages<BuyerTransferredToNewLotSaga_CleanExistingJob>,
        IHandleMessages<BuyerTransferredToNewLotSaga_EnsureNewJobExists>,
        IHandleMessages<BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks>
        //IHandleMessages<RequestJobSaleDetailsResponse>,
        //IHandleMessages<CallbackFromTIPS_GetContact>
    {
        private readonly IJobService _jobService;
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly ITaskService _taskService;

        public BuyerTransferredToNewLotSaga(IJobService jobService, IHomeOwnerService homeOwnerService, ITaskService taskService)
        {
            _jobService = jobService;
            _homeOwnerService = homeOwnerService;
            _taskService = taskService;
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<BuyerTransferredToNewLotSaga_CleanExistingJob>(x => x.ContactId).ToSaga(x => x.ContactId);
            ConfigureMapping<BuyerTransferredToNewLotSaga_EnsureNewJobExists>(x => x.ContactId).ToSaga(x => x.ContactId);
            ConfigureMapping<BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks>(x => x.ContactId).ToSaga(x => x.ContactId);
            //ConfigureMapping<CallbackFromTIPS_GetContact>(x => x.ContactId).ToSaga(x => x.ContactId);
            //ConfigureMapping<RequestJobSaleDetailsResponse>(x => x.ContactId).ToSaga(x => x.ContactId);
        }

        public void Handle(BuyerTransferredToNewLot message)
        {
            Data.NewJobNumber = message.NewJobNumber;
            Data.PreviousJobNumber = message.PreviousJobNumber;
            Data.ContactId = message.ContactId;

            var homeowner = _homeOwnerService.GetHomeOwnerByJobNumber(message.PreviousJobNumber);
            if (homeowner == null)
            {
                // Get Contact from TIPS
                //Bus.Send(new GetHomeownerInformation(Data.ContactId));
                return;
            }

            Bus.SendLocal(new BuyerTransferredToNewLotSaga_CleanExistingJob(Data.ContactId));
        }

        //public void Handle(CallbackFromTIPS_GetHomeownerInformation message) // Response from TIPS
        //{
        //    var homeOwner = new HomeOwner
        //    {
        //        HomeOwnerId = Guid.NewGuid(),
        //        CreatedBy = "Warranty.Server",
        //        CreatedDate = DateTime.UtcNow,
        //        HomeOwnerNumber = homeOwnerNumber,
        //        UpdatedBy = "Warranty.Server"
        //    };

        //    Automapper.Map<HomeOwner>(message, homeOwner);

        //    Data.NewHomeOwner = homeOwner;
        //    Bus.SendLocal(new BuyerTransferredToNewLotSaga_EnsureNewJobExists
        //    {
        //        NewJobNumber = Data.NewJobNumber,
        //        PreviousJobNumber = Data.PreviousJobNumber
        //    });
        //}

        public void Handle(BuyerTransferredToNewLotSaga_CleanExistingJob message)
        {
            var homeowner = _homeOwnerService.GetHomeOwnerByJobNumber(Data.PreviousJobNumber);
            var previousJob = _jobService.GetJobByNumber(Data.PreviousJobNumber);

            _homeOwnerService.RemoveFromJob(homeowner, previousJob);

            _taskService.DeleteTask(previousJob.JobId, TaskType.JobStage3);
            _taskService.DeleteTask(previousJob.JobId, TaskType.JobStage10);
            _taskService.CreateTasks(previousJob.JobId);

            Bus.SendLocal(new BuyerTransferredToNewLotSaga_EnsureNewJobExists(Data.ContactId));
        }

        public void Handle(BuyerTransferredToNewLotSaga_EnsureNewJobExists message)
        {
            var newJob = _jobService.GetJobByNumber(Data.NewJobNumber);

            if (newJob == null)
            {
                // Get Sale from TIPS
                // Bus.Send(new RequestJobSaleDetailsCommand(Data.NewJobNumber));
                return;
            }

            Bus.SendLocal(new BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks(Data.ContactId));
        }

        //public void Handle(RequestJobSaleDetailsResponse message) // Response from TIPS
        //{
        //    var newJob = Automapper.Map<Job>(message);
        //    Data.NewJob = _jobService.CreateJob(newJob);

        //    Bus.SendLocal(new BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks(Data.ContactId));
        //}

        public void Handle(BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks message)
        {
            _homeOwnerService.AssignToJob(Data.NewHomeOwner, Data.NewJob);
            _taskService.CreateTasks(Data.NewJob.JobId);

            MarkAsComplete();
        }
    }

    public class BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks : ICommand
    {
        public Guid ContactId { get; set; }

        public BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks() { }

        public BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks(Guid contactId)
        {
            ContactId = contactId;
        }
    }

    public class BuyerTransferredToNewLotSaga_EnsureNewJobExists : ICommand
    {
        public Guid ContactId { get; set; }

        public BuyerTransferredToNewLotSaga_EnsureNewJobExists() { }

        public BuyerTransferredToNewLotSaga_EnsureNewJobExists(Guid contactId)
        {
            ContactId = contactId;
        }
    }

    public class BuyerTransferredToNewLotSaga_CleanExistingJob : ICommand
    {
        public Guid ContactId { get; set; }

        public BuyerTransferredToNewLotSaga_CleanExistingJob() { }

        public BuyerTransferredToNewLotSaga_CleanExistingJob(Guid contactId)
        {
            ContactId = contactId;
        }
    }

    public class BuyerTransferredToNewLotSagaData : IContainSagaData
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public string NewJobNumber { get; set; }
        public string PreviousJobNumber { get; set; }
        public Guid ContactId { get; set; }
        public HomeOwner NewHomeOwner { get; set; }
        public Job NewJob { get; set; }
    }
}