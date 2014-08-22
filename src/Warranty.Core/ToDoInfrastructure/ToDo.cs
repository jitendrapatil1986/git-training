using System;
using Warranty.Core.ToDoInfrastructure.Interfaces;

namespace Warranty.Core.ToDoInfrastructure
{
    public abstract class ToDo<TModel> : IToDo
    {
        public TModel Model { get; set; }
        public abstract string ViewName { get; set; }
        public DateTime Date { get; set; }
    }
}