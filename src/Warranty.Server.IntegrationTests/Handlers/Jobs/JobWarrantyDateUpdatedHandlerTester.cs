using System;
using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobWarrantyDateUpdatedHandlerTester : HandlerTester<JobWarrantyDateUpdated>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.WarrantyDate = DateTime.Parse("01/01/2014");
            });
        }

        [Test]
        public void Job_Close_Date_Should_Be_Updated()
        {
            var payment = Get<Job>(_job.JobId);
            payment.CloseDate.ShouldEqual(Event.WarrantyDate);
        }
    }
}
