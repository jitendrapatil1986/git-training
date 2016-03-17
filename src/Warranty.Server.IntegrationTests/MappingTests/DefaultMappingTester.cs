using AutoMapper;
using NUnit.Framework;

namespace Warranty.Server.IntegrationTests.MappingTests
{
    public class DefaultMappingTester : MappingTest
    {
        [Test]
        public void ShouldHaveValidConfiguration()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}