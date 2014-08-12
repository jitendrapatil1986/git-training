using System;
using System.Linq.Expressions;
using NPoco;
using NServiceBus;
using NServiceBus.UnitOfWork;
using StructureMap;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests
{
    public abstract class CommandHandlerTester<TEvent> where TEvent : IMessage, new()
    {
        private readonly IContainer _container;
        private readonly IDatabase _database;

        protected CommandHandlerTester(IDatabase database)
        {
            _database = database;
            _container = TestIoC.Container.GetNestedContainer();

            var deleter = _container.GetInstance<DatabaseDeleter>();
            deleter.DeleteAllData(database);
        }

        protected T GetSaved<T>(Action<T> action = null)
        {
            var builder = _container.GetInstance<EntityBuilder<T>>();
            return builder.GetSaved(action ?? (x => { }));
        }

        protected TEvent Send(Action<TEvent> eventAction)
        {
            var @event = new TEvent();
            eventAction(@event);
            _container.Configure(cfg => cfg.For<IBus>().Use(Bus));
            var unitOfWork = _container.GetInstance<IManageUnitsOfWork>();
            var handler = _container.GetInstance<IHandleMessages<TEvent>>();

            try
            {
                unitOfWork.Begin();
                handler.Handle(@event);
                unitOfWork.End();
            }
            catch (Exception ex)
            {
                unitOfWork.End(ex);
                throw;
            }
            return @event;
        }

        protected IBus Bus { get; private set; }

        protected T Get<T>(object id)
        {
            using (_database)
            {
                return _database.SingleById<T>(id);
            }
        }

        protected T Get<T>(Expression<Func<T, bool>> predicate)
        {
            using (_database)
            {
                return _database.Query<T>().Where(predicate).FirstOrDefault();
            }
        }
    }
}