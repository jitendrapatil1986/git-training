using System;
using System.Collections.Generic;
using NPoco;
using NServiceBus;
using StructureMap;
using Warranty.Core.DataAccess;
using Warranty.Core.Security;
using Warranty.Server.IntegrationTests.Security;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests
{
    public abstract class HandlerTester<TEvent> where TEvent : IEvent, new()
    {
        protected readonly IContainer TestContainer;
        protected readonly IDatabase TestDatabase;

        protected HandlerTester()
        {
            TestContainer = TestIoC.Container.GetNestedContainer();

            DbFactory.Setup(new WarrantyUserSession());
            TestDatabase = DbFactory.DatabaseFactory.GetDatabase();

            var deleter = new DatabaseDeleter(TestDatabase);
            deleter.DeleteAllData(TestDatabase);
        }

        protected TEvent Event { get; set; }

        public T GetSaved<T>(Action<T> action = null)
        {
            var builder = TestContainer.GetInstance<EntityBuilder<T>>();
            var savedItem = builder.GetSaved(action ?? (x => { }));
            return savedItem;
        }

        public void Send(Action<TEvent> eventAction)
        {
            Event = new TEvent();
            eventAction(Event);

            ExecuteSend();
        }

        public void Send(TEvent @event)
        {
            Event = @event;
            ExecuteSend();
        }

        private void ExecuteSend()
        {
            var handler = TestContainer.GetInstance<IHandleMessages<TEvent>>();

            try
            {
                handler.Handle(Event);
            }
            catch {}
        }

        protected T Get<T>(object id)
        {
            using (TestDatabase)
            {
                return TestDatabase.SingleById<T>(id);
            }
        }

        protected IEnumerable<T> Query<T>()
        {
            using (TestDatabase)
            {
                return TestDatabase.Query<T>().ToList();
            }
        }
    }
}
