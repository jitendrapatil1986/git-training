using System;
using System.Linq.Expressions;
using NPoco;
using NServiceBus;
using NServiceBus.UnitOfWork;
using StructureMap;
using Warranty.Core.DataAccess;
using Warranty.Core.Security;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests
{
    public abstract class CommandHandlerTester<TEvent> where TEvent : IMessage, new()
    {
        private readonly IContainer TestContainer;
        private readonly IDatabase TestDatabase;

        protected CommandHandlerTester(IUserSession userSession)
        {
            TestContainer = TestIoC.Container.GetNestedContainer();

            DbFactory.Setup(userSession);
            TestDatabase = DbFactory.DatabaseFactory.GetDatabase();

            var deleter = TestContainer.GetInstance<DatabaseDeleter>();
            deleter.DeleteAllData(TestDatabase);
        }

        protected T GetSaved<T>(Action<T> action = null)
        {
            var builder = TestContainer.GetInstance<EntityBuilder<T>>();
            return builder.GetSaved(action ?? (x => { }));
        }

        protected TEvent Send(Action<TEvent> eventAction)
        {
            var @event = new TEvent();
            eventAction(@event);
            TestContainer.Configure(cfg => cfg.For<IBus>().Use(Bus));
            var handler = TestContainer.GetInstance<IHandleMessages<TEvent>>();

            try
            {
                handler.Handle(@event);
            }
            catch {}

            return @event;
        }

        protected IBus Bus { get; private set; }

        protected T Get<T>(object id)
        {
            using (TestDatabase)
            {
                return TestDatabase.SingleById<T>(id);
            }
        }

        protected T Get<T>(Expression<Func<T, bool>> predicate)
        {
            using (TestDatabase)
            {
                return TestDatabase.Query<T>().Where(predicate).FirstOrDefault();
            }
        }
    }
}