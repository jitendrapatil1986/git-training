using StructureMap;

namespace Warranty.Server.IntegrationTests
{
    public static class TestIoC
    {
        public static IContainer Container { get; internal set; }
    }
}