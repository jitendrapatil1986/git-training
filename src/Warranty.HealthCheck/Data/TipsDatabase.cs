using NPoco;

namespace Warranty.HealthCheck.Data
{
    public interface ITipsDatabase : IDatabase { }

    internal class TipsDatabase : Database, ITipsDatabase
    {
        public TipsDatabase(string connectionStringName) : base(connectionStringName)
        {
        }
    }
}