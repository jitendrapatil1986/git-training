using System;
using Common.Messages;
using NPoco;
using NServiceBus;
using StructureMap;
using Warranty.Server.Extensions;
using Warranty.Server.IntegrationTests.SetUp;
using Warranty.Core.Entities;
using Warranty.Tests.Core;

namespace Warranty.Server.IntegrationTests
{

    public abstract class BusHandlerTesterBase<TEvent> where TEvent : IBusEvent, new()
    {
        protected readonly IDatabase TestDatabase;

        protected BusHandlerTesterBase()
        {
            TestDatabase = ObjectFactory.GetInstance<IDatabase>();

            var deleter = new DatabaseDeleter(TestDatabase);
            deleter.DeleteAllData(TestDatabase);
        }

        protected TEvent Event { get; set; }

        public T GetSaved<T>(Action<T> action = null)
        {
            var builder = ObjectFactory.GetInstance<EntityBuilder<T>>();
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
            var handler = ObjectFactory.GetInstance<IHandleMessages<TEvent>>();
            handler.Handle(Event);
        }

        protected T Get<T>(object id)
        {
            using (TestDatabase)
                return TestDatabase.SingleById<T>(id);
        }

        protected T Get<T>(string jdeId) where T : IJdeEntity
        {
            using (TestDatabase)
                return TestDatabase.SingleOrDefaultByJdeId<T>(jdeId);
        }
    }
}