namespace Warranty.Core
{
    public interface ICommandResultHandler<in TMessage, in TResult> : IResultHandler
    {
        void Handle(TMessage message, TResult result);
    }
}