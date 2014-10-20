namespace Warranty.Core.TaskInfrastructure.Interfaces
{
    public interface ITask<T> where T : class
    {
        void Create(T entity);
    }
}
