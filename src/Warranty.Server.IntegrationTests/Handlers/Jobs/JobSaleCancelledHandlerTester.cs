﻿using System;
using System.Linq;
using NUnit.Framework;
using Should;
using StructureMap;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;
using Warranty.Server.Extensions;
using Warranty.Server.Handlers.Jobs;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobSaleCancelledHandlerTester : BusHandlerTesterBase<JobSaleCancelled>
    {
        private IHomeOwnerService _homeOwnerService;
        private ITaskService _taskService;

        public JobSaleCancelledHandlerTester()
        {
            _homeOwnerService = ObjectFactory.GetInstance<IHomeOwnerService>();
            _taskService = ObjectFactory.GetInstance<ITaskService>();
        }
        public void SendMessage(string jobNumber, Guid? contactId = null)
        {
            var message = new JobSaleCancelled
            {
                JobNumber = jobNumber
            };
            if (contactId.HasValue)
            {
                message.ContactId = contactId.Value;
            }
            Send(message);
        }

        [Test]
        public void Homeowner_Should_Be_Deleted()
        {
            var job = GetSaved<Job>();
            var homeOwner = GetSaved<HomeOwner>(h => { h.JobId = job.JobId; });

            var homeOwnersFromDb = _homeOwnerService.GetHomeOwnerByJobNumber(job.JobNumber);
            homeOwnersFromDb.ShouldNotBeNull();

            SendMessage(job.JobNumber);

            homeOwnersFromDb = _homeOwnerService.GetHomeOwnerByJobNumber(job.JobNumber);
            homeOwnersFromDb.ShouldBeNull();
        }

        [Test]
        public void Todos_Should_Be_Deleted()
        {
            var community = GetSaved<Community>();
            var job = GetSaved<Job>(j => j.CommunityId = community.CommunityId);
            var homeOwner = GetSaved<HomeOwner>(h => { h.JobId = job.JobId; });
            var wsr = GetSaved<Employee>();

            using (TestDatabase)
            {
                TestDatabase.Insert(new CommunityAssignment
                {
                    CommunityId = community.CommunityId,
                    EmployeeId = wsr.EmployeeId
                });
            }

            _taskService.CreateTaskIfDoesntExist(job.JobId, wsr.EmployeeId, TaskType.JobStage3);
            _taskService.CreateTaskIfDoesntExist(job.JobId, wsr.EmployeeId, TaskType.JobStage7);
            _taskService.CreateTaskIfDoesntExist(job.JobId, wsr.EmployeeId, TaskType.JobStage10);

            var allTasks = _taskService.GetTasksByJobId(job.JobId);
            allTasks.Count.ShouldEqual(3);
            SendMessage(job.JobNumber);

            allTasks = _taskService.GetTasksByJobId(job.JobId);
            allTasks.Count.ShouldEqual(1);
            allTasks.First().TaskType.ShouldEqual(TaskType.JobStage7);
        }
    }
}