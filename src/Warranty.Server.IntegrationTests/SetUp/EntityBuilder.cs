﻿using System;
using FizzWare.NBuilder;
using NPoco;
using StructureMap;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public abstract class EntityBuilder<T> : IEntityBuilder<T>
    {
        protected readonly IDatabase _database;

        protected EntityBuilder(IDatabase database)
        {
            _database = database;
        }

        public abstract T GetSaved(Action<T> action);

        public T GetRandom()
        {
            return Builder<T>.CreateNew().Build();
        }

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