namespace Warranty.Core
{
    using StructureMap;

    public class Mediator : IMediator
    {
        readonly IContainer _container;

        public Mediator(IContainer container)
        {
            _container = container;
        }

        public void Send(ICommand message)
        {
            var handler = GetHandler(message);
            ProcessCommandWithHandler(message, handler);
        }

        public TResult Send<TResult>(ICommand<TResult> message)
        {
            var handler = GetHandler(message);
            var result = ProcessCommandWithHandler(message, handler);

            return result;
        }

        public TResponse Request<TResponse>(IQuery<TResponse> query)
        {
            var handler = GetHandler(query);
            var result = ProcessQueryWithHandler(query, handler);

            return result;
        }

        object GetHandler<TResponse>(IQuery<TResponse> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
            var handler = _container.GetInstance(handlerType);
            return handler;
        }

        object GetHandler<TResult>(ICommand<TResult> message)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(message.GetType(), typeof(TResult));
            var handler = _container.GetInstance(handlerType);
            return handler;
        }

        object GetHandler(ICommand message)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(message.GetType());
            var handler = _container.GetInstance(handlerType);
            return handler;
        }

        TResponse ProcessQueryWithHandler<TResponse>(IQuery<TResponse> query, object handler)
        {
            return (TResponse)handler.GetType().GetMethod("Handle").Invoke(handler, new object[] { query });
        }

        TResult ProcessCommandWithHandler<TResult>(ICommand<TResult> message, object handler)
        {
            return (TResult)handler.GetType().GetMethod("Handle").Invoke(handler, new object[] { message });
        }

        void ProcessCommandWithHandler(ICommand message, object handler)
        {
            handler.GetType().GetMethod("Handle").Invoke(handler, new object[] { message });
        }
    }
}
