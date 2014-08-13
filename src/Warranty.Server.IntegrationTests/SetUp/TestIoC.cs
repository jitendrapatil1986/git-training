using StructureMap;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public static class TestIoC
    {
        public static IContainer Container { get; internal set; }
    }
}