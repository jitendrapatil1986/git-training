namespace Warranty.IntegrationTests.PersistenceTests
{
    using System;
    using Core;
    using Core.DataAccess;
    using Core.Entities;
    using Core.Security;
    using MediatorMessagingTests;
    using MediatorMessagingTests.EntityBuilders;
    using NPoco;
    using NServiceBus;
    using NUnit.Framework;
    using Rhino.Mocks;
    using StructureMap;
    using Tests.Core;
    using ICommand = Core.ICommand;

    [TestFixture]
    public abstract class MediatorMessagingTesterBase 
    {
        protected readonly IDatabase TestDatabase;
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            DbFactory.Setup(ObjectFactory.Container);

            var deleter = new DatabaseDeleter(DbFactory.DatabaseFactory.GetDatabase());
            deleter.DeleteAllData(DbFactory.DatabaseFactory.GetDatabase());
        }

        protected MediatorMessagingTesterBase()
        {
            TestDatabase = ObjectFactory.GetInstance<IDatabase>();

            var deleter = new DatabaseDeleter(TestDatabase);
            deleter.DeleteAllData(TestDatabase);
        }
        protected void Send(ICommand command)
        {
            var mediator = ObjectFactory.GetInstance<IMediator>();
            Bus = MockRepository.GenerateStub<IBus>();
            ObjectFactory.Configure(cfg => cfg.For<IBus>().Use(Bus));
            mediator.Send(command);
        }

        protected TModel Send<TModel>(ICommand<TModel> command)
        {
            var mediator = ObjectFactory.GetInstance<IMediator>();
            Bus = MockRepository.GenerateStub<IBus>();
            ObjectFactory.Configure(cfg => cfg.For<IBus>().Use(Bus));
            return mediator.Send(command);
        }

        public TResponse Request<TResponse>(IQuery<TResponse> query)
        {
            var mediator = ObjectFactory.GetInstance<IMediator>();
            return mediator.Request(query);
        }

        public T GetSaved<T>(Action<T> action = null)
        {
            var builder = ObjectFactory.GetInstance<EntityBuilder<T>>();
            var savedItem = builder.GetSaved(action ?? (x => { }));
            return savedItem;
        }

        public IBus Bus { get; set; }

        protected T Get<T>(object id)
        {
            using (TestDatabase)
            {
                return TestDatabase.SingleById<T>(id);
            }
        }

        protected TEntity Insert<TEntity>(TEntity entity)
        {
            using (var db = ObjectFactory.GetInstance<IDatabase>())
            {
                db.Insert(entity);
                return entity;
            }
        }

        protected TEntity Update<TEntity>(TEntity entity)
        {
            using (var db = ObjectFactory.GetInstance<IDatabase>())
            {
                db.Update(entity);
                return entity;
            }
        }

        protected TEntity Load<TEntity>(Guid id)
        {
            using (var db = ObjectFactory.GetInstance<IDatabase>())
            {
                return db.SingleOrDefaultById<TEntity>(id);
            }
        }
    }
}
