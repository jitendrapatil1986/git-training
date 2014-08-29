using System;
using System.Linq.Expressions;
using NPoco;
using NServiceBus;
using StructureMap;
using Warranty.Core.DataAccess;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests
{
    using Tests.Core;

    public abstract class CommandHandlerTester<TEvent> where TEvent : IMessage, new()
    {
        private readonly IDatabase TestDatabase;

        protected CommandHandlerTester()
        {
            TestDatabase = DbFactory.DatabaseFactory.GetDatabase();

            var deleter = ObjectFactory.GetInstance<DatabaseDeleter>();
            deleter.DeleteAllData(TestDatabase);
        }

        protected T GetSaved<T>(Action<T> action = null)
        {
            var builder = ObjectFactory.GetInstance<EntityBuilder<T>>();
            return builder.GetSaved(action ?? (x => { }));
        }

        protected TEvent Send(Action<TEvent> eventAction)
        {
            var @event = new TEvent();
            eventAction(@event);
            ObjectFactory.Configure(cfg => cfg.For<IBus>().Use(Bus));
            var handler = ObjectFactory.GetInstance<IHandleMessages<TEvent>>();

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