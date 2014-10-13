using System;
using Warranty.Core.Enumerations;

namespace Warranty.Core.ToDoInfrastructure.Interfaces
{
    public interface IToDo
    {
        object ViewModel { get; }
        string ViewName { get; }
        DateTime Date { get; set; }
        ToDoType Type { get; }
    }
}