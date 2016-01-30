using StructureMap;

namespace Warranty.Server.IntegrationTests.Extensions.IDatabase
{
    public abstract class DBTestBase
    {
        protected readonly NPoco.IDatabase TestDatabase;

        public DBTestBase()
        {
            TestDatabase = ObjectFactory.GetInstance<NPoco.IDatabase>();
        }
    }
}
