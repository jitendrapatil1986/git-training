namespace Warranty.Core
{
    public interface IQueryHandler<in TQuery, out TResponse> : IHandler
        where TQuery : IQuery<TResponse>
    {
        TResponse Handle(TQuery query);
    }
}