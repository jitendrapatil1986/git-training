namespace Warranty.IntegrationTests
{
    using NUnit.Framework;
    using StructureMap;
    using Core;

    [SetUpFixture]
    public class IntegrationSetUpFixture
    {
        public IntegrationSetUpFixture()
        {
            ObjectFactory.Initialize(x => x.AddRegistry<WarrantyCoreRegistry>());
        }
    }
}