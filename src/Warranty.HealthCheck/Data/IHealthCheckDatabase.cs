using NPoco;

namespace Warranty.HealthCheck.Data
{
    public interface IHealthCheckDatabase : IDatabase { }

    public class HealthCheckDatabase : Database, IHealthCheckDatabase
    {
        public HealthCheckDatabase(string connectionStringName) : base(connectionStringName)
        {
        }
    }
}