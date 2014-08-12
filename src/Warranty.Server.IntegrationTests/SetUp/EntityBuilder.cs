using System;
using NPoco;
using StructureMap;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public abstract class EntityBuilder<T> : IEntityBuilder<T>
    {
        private readonly IContainer _container;
        private readonly IDatabase _database;

        protected EntityBuilder(IDatabase database)
        {
            _container = TestIoC.Container;
            _database = database;
        }

        public abstract T GetSaved(Action<T> action);

        public T Saved(T entity, Action<T> action)
        {
            if (action != null) action(entity);
            {
                using (_database)
                {
                    _database.Save<T>(entity);
                }
            }

            return entity;
        }

        public TEntity GetSaved<TEntity>()
        {
            var builder = _container.GetInstance<EntityBuilder<TEntity>>();
            return builder.GetSaved(null);
        }
    }
}