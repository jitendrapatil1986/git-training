using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobPrimaryBuilderUpdatedHandlerExistingBuilderTester : HandlerTester<JobPrimaryBuilderUpdated>
    {
        private Job _job;
        private Employee _originalBuilder;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _originalBuilder = GetSaved<Employee>();
            _job = GetSaved<Job>();

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.PrimaryBuilderName = _originalBuilder.Name;
                x.PrimaryBuilderNumber = _originalBuilder.JdeIdentifier;
            });
        }

        [Test]
        public void Job_PrimaryBuilder_Should_Be_Updated()
        {
            var job = Get<Job>(Event.JDEId);
            job.BuilderEmployeeId.ShouldEqual(_originalBuilder.EmployeeId);
        }
    }
}
