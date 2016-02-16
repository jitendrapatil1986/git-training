using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using NUnit.Framework;
using Should;
using Should.Core.Exceptions;
using TIPS.Events.JobEvents;
using TIPS.Events.Models;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Job = Warranty.Core.Entities.Job;
using Task = Warranty.Core.Entities.Task;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class ShowcaseApprovedHandlerTester : HandlerTester<ShowcaseApproved>
    {
        private void AssignWsrToCommunity(string communityNumber)
        {
            using (TestDatabase)
            {
                var community = TestDatabase.First<Community>(string.Format("WHERE CommunityNumber = '{0}'", communityNumber.Substring(0,4)));
                var wsr = GetSaved<Employee>();
                TestDatabase.Insert(new CommunityAssignment
                {
                    CommunityId = community.CommunityId,
                    EmployeeId = wsr.EmployeeId
                });
            }
        }

        [Test]
        public void ShowcaseAdded_JobExists()
        {
            CreateShowcaseAndSendThenAssertSameness(x =>
            {
                var job = GetSaved<Job>();
                x.JobNumber = job.JobNumber;
            });
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ShowcaseAdded_HomeownerExistsThrowsException()
        {
            CreateShowcaseAndSendThenAssertSameness(x =>
            {
                var job = GetSaved<Job>();
                var homeowner = GetSaved<HomeOwner>(h => h.JobId = job.JobId);
                job.CurrentHomeOwnerId = homeowner.HomeOwnerId;
                using (TestDatabase)
                {
                    TestDatabase.Update(job);
                }
                x.JobNumber = job.JobNumber;
            });
        }

        [Test]
        public void ShowcaseAdded_Stage10ShouldNotGenerateToDo()
        {
            CreateShowcaseAndSendThenAssertSameness(x =>
            {
                x.Stage = 10;
                AssignWsrToCommunity(x.CommunityNumber);
            },
            (job, showcase) =>
            {
                using (TestDatabase)
                {
                    var todo = TestDatabase.Fetch<Task>(string.Format("WHERE ReferenceId = '{0}'", job.JobId));
                    todo.Count.ShouldEqual(0);
                }
            });
        }

        [Test]
        public void ShowcaseAdded_Stage7ShouldGenerateToDo()
        {
            CreateShowcaseAndSendThenAssertSameness(x =>
            {
                x.Stage = 7;
                AssignWsrToCommunity(x.CommunityNumber);
            },
            (job, showcase) =>
            {
                using (TestDatabase)
                {
                    var todo = TestDatabase.First<Task>(string.Format("WHERE ReferenceId = '{0}'", job.JobId));
                    todo.ShouldNotBeNull();

                    todo.Description.ShouldEqual(TaskType.JobStage7.DisplayName);
                    todo.TaskType.ShouldEqual(TaskType.JobStage7);
                }
            });

        }

        [Test]
        public void ShowcaseAdded_NullStageShouldBe0()
        {
            CreateShowcaseAndSendThenAssertSameness(x =>
            {
                x.Stage = null;
            });
        }

        [Test]
        public void ShowcaseAdded_WithLegalDescription()
        {
            CreateShowcaseAndSendThenAssertSameness(x =>
            {
                x.LegalDescription = new LegalDescription
                {
                    Phase = "TestPhase",
                    Lot = "TestLot",
                    Block = "TestBlock",
                    Section = "TestSection"
                };
            });
        }

        [Test]
        public void ShowcaseShouldBeAdded()
        {
            CreateShowcaseAndSendThenAssertSameness(x => { });
        }

        private Showcase CreateShowcase()
        {
            var community = GetSaved<Community>();
            Random rand = new Random();
            var builderEmployeeNumber = rand.Next(999999);
            var designerEmployeeNumber = rand.Next(999999);
            var communityNumber = community.CommunityNumber.Substring(0, 4);
            var jobNumber = communityNumber + rand.Next(9999);
            var stateCode = "tx";
            var zipCode = "77429";
            var planType = "s";
            var planNumber = rand.Next(9999);
            var elevation = "A";

            var builder = GetSaved<Employee>(e => e.Number = builderEmployeeNumber.ToString());
            var designer = GetSaved<Employee>(e => e.Number = designerEmployeeNumber.ToString());

            var showcase = Builder<Showcase>.CreateNew()
                .With(x => x.CommunityNumber = communityNumber.PadRight(8, '0'))
                .With(x => x.BuilderEmployeeID = builderEmployeeNumber)
                .With(x => x.DesignerEmployeeID = designerEmployeeNumber)
                .With(x => x.AddressStateAbbreviation = stateCode)
                .With(x => x.AddressZipCode = zipCode)
                .With(x => x.JobNumber = jobNumber)
                .With(x => x.JobType = planType)
                .With(x => x.PlanNumber = planNumber.ToString())
                .With(x => x.Elevation = elevation)
                .Build();

            return showcase;
        }

        private void CreateShowcaseAndSendThenAssertSameness(Action<Showcase> modifyShowcaseAction, Action<Job, Showcase> additionalAssertionAction = null)
        {
            var showcase = CreateShowcase();
            showcase.Stage = null;
            modifyShowcaseAction(showcase);
            Send(new ShowcaseApproved
            {
                Showcase = showcase
            });

            Job showcaseFromDb = null;
            using (TestDatabase)
            {
                showcaseFromDb = TestDatabase.First<Job>(string.Format("WHERE JobNumber = '{0}'", showcase.JobNumber));
            }
            AssertShowcaseAndJobAreSame(showcaseFromDb, showcase);
            if (additionalAssertionAction != null) additionalAssertionAction(showcaseFromDb, showcase);
        }

        private void AssertShowcaseAndJobAreSame(Job job, Showcase showcase)
        {
            job.JobNumber.ShouldEqual(showcase.JobNumber);
            job.PlanNumber.ShouldEqual(showcase.PlanNumber);
            job.Elevation.ShouldEqual(showcase.Elevation);
            job.AddressLine.ShouldEqual(showcase.AddressLine1);
            job.City.ShouldEqual(showcase.AddressCity);
            job.StateCode.ShouldEqual(showcase.AddressStateAbbreviation);
            job.PostalCode.ShouldEqual(showcase.AddressZipCode);
            job.PlanType.ShouldEqual(showcase.JobType);
            job.CreatedBy.ShouldEqual("Warranty.Server");
            job.JdeIdentifier.ShouldEqual(showcase.JobNumber);
            job.PlanName.ShouldEqual(showcase.PlanName);
            job.PlanTypeDescription.ShouldBeNull();
            job.WarrantyExpirationDate.ShouldBeNull();
            job.Swing.ShouldEqual(showcase.Swing);

            if (job.LegalDescription != null)
            {
                job.LegalDescription.ShouldEqual(showcase.LegalDescription.ToString());
            }
            else
            {
                showcase.LegalDescription.ShouldBeNull();
            }

            if (showcase.Stage == null)
            {
                job.Stage.ShouldEqual(0);
            }
            else
            {
                job.Stage.ShouldEqual(showcase.Stage.Value);
            }

            using (TestDatabase)
            {
                var community = TestDatabase.First<Community>(string.Format("WHERE CommunityId = '{0}'", job.CommunityId));
                community.ShouldNotBeNull();
                community.CommunityNumber.ShouldEqual(showcase.CommunityNumber.Substring(0,4));

                if (job.BuilderEmployeeId != null)
                {
                    var builder =
                        TestDatabase.First<Employee>(string.Format("WHERE EmployeeId = '{0}'", job.BuilderEmployeeId));
                    builder.ShouldNotBeNull();
                    builder.Number.ShouldEqual(showcase.BuilderEmployeeID.ToString());
                }
                else
                {
                    showcase.BuilderEmployeeID.ShouldBeNull();
                }
            }
        }
    }
}
