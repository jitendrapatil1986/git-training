using System;
using Warranty.Core.Enumerations;

namespace Warranty.Core.ToDoInfrastructure.Interfaces
{
    public interface IToDo
    {
        string ViewName { get; }
        DateTime Date { get; set; }
        ToDoType Type { get; }
    }
}