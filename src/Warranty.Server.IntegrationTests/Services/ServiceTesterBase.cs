using System;
using StructureMap;
using Warranty.Server.IntegrationTests.SetUp;
using Warranty.Tests.Core;

namespace Warranty.Server.IntegrationTests.Services
{
    public abstract class ServiceTesterBase
    {
        protected readonly NPoco.IDatabase TestDatabase;

        public ServiceTesterBase()
        {
            TestDatabase = ObjectFactory.GetInstance<NPoco.IDatabase>();
            var deleter = new DatabaseDeleter(TestDatabase);
            deleter.DeleteAllData(TestDatabase);
        }

        public T Get<T>()
        {
            return ObjectFactory.GetInstance<EntityBuilder<T>>().GetSaved<T>();
        }

        public T Get<T>(Action<T> action)
        {
            return ObjectFactory.GetInstance<EntityBuilder<T>>().GetSaved(action);
        }
    }
}
