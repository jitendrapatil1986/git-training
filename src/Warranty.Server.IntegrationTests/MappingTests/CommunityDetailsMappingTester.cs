using AutoMapper;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Core.Services.Models;

namespace Warranty.Server.IntegrationTests.MappingTests
{
    [TestFixture]
    public class CommunityDetailsMappingTester : MappingTest
    {
        [Test]
        public void ShouldMapEmptyDetails()
        {
            var details = new CommunityDetails();
            var community = Mapper.Map<Community>(details);
            community.ShouldNotBeNull();
        }
    }
}