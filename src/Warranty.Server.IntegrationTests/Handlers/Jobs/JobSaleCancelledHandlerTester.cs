using System;
using NUnit.Framework;
using Should;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobSaleCancelledHandlerTester : IBusHandlerTesterBase<JobSaleCancelled>
    {
        public void SendMessage(string jobNumber, Guid? contactId = null)
        {
            var message = new JobSaleCancelled
            {
                JobNumber = jobNumber
            };
            if (contactId.HasValue) message.ContactId = contactId.Value;
            Send(message);
        }

        
        [Test]
        public void Homeowner_Should_Be_Deleted()
        {
            var job = GetSaved<Job>();
            var homeowner = GetSaved<HomeOwner>(h =>
            {
                h.JobId = job.JobId;
            });

            var homeOwnersFromDb = TestDatabase.GetHomeOwnersByJobNumber(job.JobNumber);
            homeOwnersFromDb.ShouldNotBeNull();
            homeOwnersFromDb.Count.ShouldEqual(1);

            SendMessage(job.JobNumber); 

            homeOwnersFromDb = TestDatabase.GetHomeOwnersByJobNumber(job.JobNumber);
            homeOwnersFromDb.Count.ShouldEqual(0);
        }

    }
}