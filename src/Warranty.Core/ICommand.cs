namespace Warranty.Core
{
    public interface ICommand<out TResult> : ICommand { }

    public interface ICommand { }
}