using System;
using NUnit.Framework;
using Warranty.Core.Entities;

namespace Warranty.Tests.Entities
{
    [TestFixture]
    public class JobTests
    {

        private readonly Job _job = new Job { CloseDate = new DateTime(2013, 4, 15) };

        [Test]
        public void Should_calculate_call_was_created_during_warranty_period()
        {
            var serviceCall = new ServiceCall { CreatedDate = new DateTime(2014, 04, 15) };

        Assert.True(_job.WasCreatedDuringWarrantablePeriod(serviceCall));
        }

        [Test]
        public void Should_calculate_call_was_not_created_during_warranty_period()
        {
            var serviceCall = new ServiceCall { CreatedDate = new DateTime(2016, 04, 15) };

            Assert.False(_job.WasCreatedDuringWarrantablePeriod(serviceCall));
        }
    }
}