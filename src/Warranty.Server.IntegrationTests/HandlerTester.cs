using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;
using NServiceBus;
using NServiceBus.UnitOfWork;
using StructureMap;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests
{
    public abstract class HandlerTester<TEvent> where TEvent : IEvent, new()
    {
        protected readonly IContainer TestContainer;
        protected IDatabase _database;

        protected HandlerTester(IDatabase database)
        {
            TestContainer = TestIoC.Container.GetNestedContainer();
            _database = database;

            var deleter = TestContainer.GetInstance<DatabaseDeleter>();
            deleter.DeleteAllData(_database);
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
            var unitOfWork = TestContainer.GetInstance<IManageUnitsOfWork>();
            var handler = TestContainer.GetInstance<IHandleMessages<TEvent>>();

            try
            {
                unitOfWork.Begin();
                handler.Handle(Event);
                unitOfWork.End();
            }
            catch (Exception ex)
            {
                unitOfWork.End(ex);
                throw;
            }
        }

        protected T Get<T>(object id)
        {
            using (_database)
            {
                return _database.SingleById<T>(id);
            }
        }

        protected IEnumerable<T> Query<T>()
        {
            using (_database)
            {
                return _database.Query<T>().ToList();
            }
        }
    }
}
