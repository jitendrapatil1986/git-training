using StructureMap;

namespace Warranty.HealthCheck.Config
{
    public class StructuremapConfig
    {
        public static IContainer Init()
        {
            var container = new Container(cfg =>
            {
                cfg.AddRegistry<HealthCheckRegistry>();
            });

            return container;
        }
    }
}