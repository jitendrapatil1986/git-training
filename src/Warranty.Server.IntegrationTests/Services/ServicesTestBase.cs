using StructureMap;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests.Extensions.IDatabase
{
    public abstract class ServicesTestBase
    {
        protected readonly NPoco.IDatabase TestDatabase;

        public ServicesTestBase()
        {
            TestDatabase = ObjectFactory.GetInstance<NPoco.IDatabase>();
        }

        public T Get<T>()
        {
            return ObjectFactory.GetInstance<EntityBuilder<T>>().GetSaved<T>();
        }
    }
}
