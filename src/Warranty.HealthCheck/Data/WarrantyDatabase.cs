using NPoco;

namespace Warranty.HealthCheck.Data
{
    public interface IWarrantyDatabase : IDatabase {  }

    internal class WarrantyDatabase : Database, IWarrantyDatabase
    {
        public WarrantyDatabase(string connectionStringName) : base(connectionStringName)
        {
        }
    }
}