namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    using Accounting.Events.Job;
    using Core.Entities;
    using NUnit.Framework;
    using Should;

    [TestFixture]
    public class JobPrimaryBuilderUpdatedHandlerNewBuilderTester : HandlerTester<JobPrimaryBuilderUpdated>
    {
        private Job _job;
        private Employee _originalBuilder;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _originalBuilder = GetSaved<Employee>();
            _job = GetSaved<Job>(x => x.BuilderEmployeeId = _originalBuilder.EmployeeId);

            Send(x =>
                     {
                         x.JDEId = _job.JdeIdentifier;
                         x.PrimaryBuilderName = "New Builder";
                         x.PrimaryBuilderNumber = "9912345";
                     });
        }

        [Test]
        public void Job_PrimaryBuilder_Should_Be_Updated_Builder_Should_Be_Inserted()
        {
            var job = Get<Job>(_job.JobId);
            var employee = Get<Employee>(job.BuilderEmployeeId);

            employee.Number.ShouldEqual(Event.PrimaryBuilderNumber);
            employee.Name.ShouldEqual(Event.PrimaryBuilderName);
        }
    }
}