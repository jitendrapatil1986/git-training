using System;
using NPoco;
using NUnit.Framework;
using Should;
using StructureMap;
using Warranty.Core.Entities;
using Warranty.Core.Services;

namespace Warranty.IntegrationTests.PersistenceTests
{
    [TestFixture]
    public class CommunityServiceTester : PersistenceTesterBase
    {
        [Test]
        public void ShouldSaveCommunity()
        {
            var community = new Community();
            community.CommunityName = "test";
            community.CommunityNumber = "Test";
            community.CommunityStatusCode = "1234";

            var service = new CommunityService(ObjectFactory.GetInstance<IDatabase>());

            var result = service.Create(community);
            result.ShouldNotBeNull();
            result.CommunityId.ShouldNotEqual(Guid.Empty);
        }
    }
}