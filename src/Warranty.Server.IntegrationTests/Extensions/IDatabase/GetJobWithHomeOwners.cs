using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FizzWare.NBuilder;
using FluentAssertions;
using NPoco;
using NUnit.Framework;
using Should;
using StructureMap;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests.Extensions.IDatabase
{
    [TestFixture]
    public class GetHomeOwnersByJobNumberTester : DBTestBase
    {
        public Job GetJob()
        {
            return ObjectFactory.GetInstance<EntityBuilder<Job>>().GetSaved<Job>();
        }

        public List<HomeOwner> GetHomeOwners(int numberOfHomeOwners, Job job)
        {
            if (numberOfHomeOwners == 0)
                return null;

            var toReturn = new List<HomeOwner>(numberOfHomeOwners);
            for (var i = 0; i < numberOfHomeOwners; i++)
            {
                // Without the following wait, it generates the same guid for the IDs and fails because of primary key constraints
                System.Threading.Thread.Sleep(50);

                toReturn.Add(ObjectFactory.GetInstance<EntityBuilder<HomeOwner>>().GetSaved(x =>
                {
                    x.JobId = job.JobId;
                }));
            }

            job.CurrentHomeOwnerId = toReturn[0].HomeOwnerId;
            using (TestDatabase)
            {
                TestDatabase.Update(job);
            }
            return toReturn;
        }

        [Test]
        public void Homeowners_by_job_number_query_test()
        {
            var job = ObjectFactory.GetInstance<EntityBuilder<Job>>().GetSaved<Job>();
            var homeowners = GetHomeOwners(5, job);

            using (TestDatabase)
            {
                var allHomeOwners = TestDatabase.GetHomeOwnersByJobNumber(job.JobNumber);

                job.CurrentHomeOwnerId = null;
                TestDatabase.Update(job);
                allHomeOwners.ForEach(x => TestDatabase.Delete<HomeOwner>(x));
                homeowners.ForEach(a => allHomeOwners.Should().ContainSingle(x => x.HomeOwnerId == a.HomeOwnerId));
                TestDatabase.Delete(job);
            }
        }
    }
}