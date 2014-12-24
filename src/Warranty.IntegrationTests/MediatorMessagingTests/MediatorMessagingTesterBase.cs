namespace Warranty.IntegrationTests.PersistenceTests
{
    using System;
    using Core;
    using Core.Entities;
    using MediatorMessagingTests;
    using NPoco;
    using NUnit.Framework;
    using StructureMap;
    using Tests.Core;

    [TestFixture]
    public abstract class MediatorMessagingTesterBase : PersistenceTesterBase
    {
        protected readonly IDatabase TestDatabase;

        protected MediatorMessagingTesterBase()
        {
            TestDatabase = ObjectFactory.GetInstance<IDatabase>();

            var deleter = new DatabaseDeleter(TestDatabase);
            deleter.DeleteAllData(TestDatabase);
        }
        protected void Send(ICommand command)
        {
            var mediator = ObjectFactory.GetInstance<IMediator>();
            mediator.Send(command);
        }

        protected TModel Send<TModel>(ICommand<TModel> command)
        {
            var mediator = ObjectFactory.GetInstance<IMediator>();
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

        protected T Get<T>(object id)
        {
            using (TestDatabase)
            {
                return TestDatabase.SingleById<T>(id);
            }
        }
    }
}
