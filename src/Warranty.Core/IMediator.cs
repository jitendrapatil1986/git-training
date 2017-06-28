

namespace Warranty.Core
{
    public interface IMediator
    {
        void Send(ICommand message);
        TResult Send<TResult>(ICommand<TResult> message);
        TResponse Request<TResponse>(IQuery<TResponse> query);
        
    }
}