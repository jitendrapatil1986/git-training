namespace Warranty.Core
{
    public interface ICommandHandler<in TMessage> : IHandler
        where TMessage : ICommand
    {
        void Handle(TMessage message);
    }

    public interface ICommandHandler<in TMessage, out TResult> : IHandler
        where TMessage : ICommand<TResult>
    {
        TResult Handle(TMessage message);
    }
}