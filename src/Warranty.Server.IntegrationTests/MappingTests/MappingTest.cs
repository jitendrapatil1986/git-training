using AutoMapper;
using NUnit.Framework;

namespace Warranty.Server.IntegrationTests.MappingTests
{
    public abstract class MappingTest
    {
        [TestFixtureSetUp]
        public virtual void Setup()
        {
            Mapper.Initialize(m => m.AddProfile(new MappingProfile()));
        }
    }
}