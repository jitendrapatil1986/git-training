namespace Warranty.IntegrationTests.PersistenceTests
{
    using Core;
    using NUnit.Framework;
    using StructureMap;

    [TestFixture]
    public abstract class MediatorMessagingTesterBase : PersistenceTesterBase
    {
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
    }
}
