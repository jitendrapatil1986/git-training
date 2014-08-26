using System.Collections.Generic;
using Warranty.Core.ToDoInfrastructure.Interfaces;

namespace Warranty.Core.ToDoInfrastructure
{
    public interface IToDoAggregator
    {
        List<IToDo> Execute();
    }
}