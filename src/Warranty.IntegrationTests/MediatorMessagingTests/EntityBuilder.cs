namespace Warranty.IntegrationTests.MediatorMessagingTests
{
    using System;
    using NPoco;
    using StructureMap;

    public abstract class EntityBuilder<T> : IEntityBuilder<T>
    {
        private readonly IDatabase _database;

        protected EntityBuilder(IDatabase database)
        {
            _database = database;
        }

        public abstract T GetSaved(Action<T> action);

        public T Saved(T entity, Action<T> action)
        {
            if (action != null) action(entity);
            {
                using (_database)
                {
                    _database.Insert(entity);
                }
            }

            return entity;
        }

        public TEntity GetSaved<TEntity>()
        {
            var builder = ObjectFactory.GetInstance<EntityBuilder<TEntity>>();
            return builder.GetSaved(null);
        }
    }
}