//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FizzWare.NBuilder.Generators;
//using NUnit.Framework;
//using Should;
//using TIPS.Events.JobEvents;
//using TIPS.Events.Models;
//using Warranty.Core.Entities;
//using Warranty.Core.Enumerations;
//using Job = Warranty.Core.Entities.Job;
//using Task = Warranty.Core.Entities.Task;

//namespace Warranty.Server.IntegrationTests.Handlers.Jobs
//{
//    [TestFixture]
//    public class BuyerTransferApprovedHandlerTester : HandlerTester<BuyerTransferApproved>
//    {
//        private Community _community;
//        private Employee _wsr;

//        private delegate void ModifyMessage(BuyerTransferApproved message);
//        private delegate void Entities(Job oldJob, Job newJob, HomeOwner homeowner);

//        private void SendMessage(Entities afterSend, ModifyMessage modifyMessage = null, Entities beforeSend = null)
//        {
//            var oldJob = GetSaved<Job>();
//            var homeowner = GetSaved<HomeOwner>(h => h.JobId = oldJob.JobId);
//            var newJob = GetSaved<Job>();

//            using (TestDatabase)
//            {
//                oldJob.CurrentHomeOwnerId = homeowner.HomeOwnerId;
//                oldJob.CommunityId = _community.CommunityId;
//                TestDatabase.Update(oldJob);

//                newJob.CommunityId = _community.CommunityId;
//                TestDatabase.Update(newJob);
//            }

//            if(beforeSend != null) beforeSend(oldJob, newJob, homeowner);
//            var message = new BuyerTransferApproved
//            {
//                Opportunity = new Opportunity(),
//                Sale = new Sale
//                {
//                    JobNumber = newJob.JobNumber
//                },
//                PreviousJobNumber = oldJob.JobNumber
//            };
//            if(modifyMessage!= null) modifyMessage(message);
//            Send(message);

//            using (TestDatabase)
//            {
//                var newJobFromDb = TestDatabase.SingleById<Job>(newJob.JobId);
//                var homeownerFromDb = TestDatabase.SingleById<HomeOwner>(homeowner.HomeOwnerId);
//                var oldJobFromDb = TestDatabase.SingleById<Job>(oldJob.JobId);

//                afterSend(oldJobFromDb, newJobFromDb, homeownerFromDb);
//            }
//        }

//        private void AssertNewJobToDoExistsForStage(int stage, TaskType taskType)
//        {
//            SendMessage(beforeSend: (oldJob, newJob, homeowner) =>
//            {
//                newJob.Stage = stage;
//                TestDatabase.Update(newJob);
//            }, afterSend: (oldJob, newJob, homeowner) =>
//            {
//                var tasks = TestDatabase.Fetch<Task>(string.Format("WHERE ReferenceId = '{0}'", newJob.JobId));
//                tasks.Count.ShouldEqual(1);
//                tasks[0].TaskType.ShouldEqual(taskType);
//            });
//        }

//        private Task GetTask(Guid jobId, TaskType taskType)
//        {
//            using (TestDatabase)
//            {
//                return
//                    TestDatabase.SingleOrDefault<Task>(string.Format("WHERE ReferenceId = '{0}' AND TaskType = {1}",
//                        jobId, taskType.Value));
//            }
//        }

//        public BuyerTransferApprovedHandlerTester()
//        {
//            _community = GetSaved<Community>();
//            _wsr = GetSaved<Employee>();

//            using (TestDatabase)
//            {
//                TestDatabase.Insert(new CommunityAssignment
//                {
//                    CommunityId = _community.CommunityId,
//                    EmployeeId = _wsr.EmployeeId
//                });
//            }
//        }

//        [Test]
//        public void BuyerTransferApprovedHandler_ProperlyTurnsOldJobIntoShowcase()
//        {
//            SendMessage((oldJob, newJob, homeowner) =>
//            {
//                oldJob.CurrentHomeOwnerId.ShouldBeNull();
//            });
//        }

//        [Test]
//        public void BuyerTransferApprovedHandler_ProperlyCreatesNewJobIfDoesntExist()
//        {
//            var community = GetSaved<Community>();
//            var builder = GetSaved<Employee>();
//            var salesman = GetSaved<Employee>();

//            SendMessage(modifyMessage: (message) =>
//            {
//                message.Sale = new Sale
//                {
//                    JobNumber = "123455",
//                    CloseDate = DateTime.Today,
//                    CommunityNumber = community.CommunityNumber,
//                    BuilderEmployeeID = Int32.Parse(builder.Number),
//                    SalesConsultantEmployeeID = Int32.Parse(salesman.Number),
//                    CommunityName = community.CommunityName
//                };
//            }, afterSend: (job, newJob, homeowner) =>
//            {
//                newJob.ShouldNotBeNull();
//            });
//        }

//        [Test]
//        public void BuyerTransferApprovedHandler_ProperlyCreatesStage7TodoOnOldJob()
//        {
//            SendMessage(beforeSend: (oldJob, newJob, homeowner) =>
//            {
//                oldJob.Stage = 7;
//                TestDatabase.Update(oldJob);
//            }, afterSend: (oldJob, newJob, homeowner) =>
//            {
//                GetTask(oldJob.JobId, TaskType.JobStage7).ShouldNotBeNull();
//            });
//        }

//        [Test]
//        public void BuyerTransferApprovedHandler_ProperlyRemovesStage10TodoFromOldJob()
//        {
//            SendMessage(beforeSend: (oldJob, newJob, homeowner) =>
//            {
//                TestDatabase.Insert(new Task
//                {
//                    EmployeeId = _wsr.EmployeeId,
//                    ReferenceId = oldJob.JobId,
//                    TaskType = TaskType.JobStage10
//                });
//                GetTask(oldJob.JobId, TaskType.JobStage10).ShouldNotBeNull();
//            }, afterSend: (oldJob, newJob, homeowner) =>
//            {
//                GetTask(oldJob.JobId, TaskType.JobStage3).ShouldBeNull();
//            });
//        }

//        [Test]
//        public void BuyerTransferApprovedHandler_ProperlyRemovesStage3TodoFromOldJob()
//        {
//            SendMessage(beforeSend: (oldJob, newJob, homeowner) =>
//            {
//                TestDatabase.Insert(new Task
//                {
//                    EmployeeId = _wsr.EmployeeId,
//                    ReferenceId = oldJob.JobId,
//                    TaskType = TaskType.JobStage3
//                });
//                GetTask(oldJob.JobId, TaskType.JobStage3).ShouldNotBeNull();
//            }, afterSend: (oldJob, newJob, homeowner) =>
//            {
//                GetTask(oldJob.JobId, TaskType.JobStage3).ShouldBeNull();
//            });
//        }

//        [Test]
//        public void BuyerTransferApprovedHandler_ProperlyCreatesTaskOnNewJobStage3()
//        {
//            AssertNewJobToDoExistsForStage(3, TaskType.JobStage3);
//        }

//        [Test]
//        public void BuyerTransferApprovedHandler_ProperlyCreatesTaskOnNewJobStage7()
//        {
//            AssertNewJobToDoExistsForStage(7, TaskType.JobStage7);
//        }

//        [Test]
//        public void BuyerTransferApprovedHandler_ProperlyCreatesTaskOnNewJobStage10()
//        {
//            AssertNewJobToDoExistsForStage(10, TaskType.JobStage10);
//        }

//        [Test]
//        public void BuyerTransferApprovedHandler_ProperlyTransfersBuyerToNewJob()
//        {
//            SendMessage((oldJob, newJob, homeowner) =>
//            {
//                oldJob.CurrentHomeOwnerId.ShouldBeNull();
//                newJob.CurrentHomeOwnerId.ShouldEqual(homeowner.HomeOwnerId);
//                homeowner.JobId.ShouldEqual(newJob.JobId);
//            });
//        }
//    }
//}
