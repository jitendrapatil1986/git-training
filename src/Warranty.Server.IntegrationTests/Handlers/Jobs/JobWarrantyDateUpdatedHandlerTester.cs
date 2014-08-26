using System;
using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobWarrantyDateUpdatedHandlerTester : HandlerTester<JobWarrantyDateUpdated>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var community = GetSaved<Community>();
            _job = GetSaved<Job>(j => j.CommunityId = community.CommunityId);

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.WarrantyDate = DateTime.Parse("01/01/2014");
            });
        }

        [Test]
        public void Job_Close_Date_Should_Be_Updated()
        {
            var job = Get<Job>(_job.JobId);
            job.CloseDate.ShouldEqual(Event.WarrantyDate);
            job.WarrantyExpirationDate.ShouldEqual(Event.WarrantyDate.AddYears(10));
        }
    }
}
