using System;
using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure.Interfaces;

namespace Warranty.Core.ToDoInfrastructure
{
    public abstract class ToDo<TModel> : IToDo
    {
        public TModel Model { get; set; }

        public object ViewModel { get { return GetType().GetProperty("Model").GetValue(this); }}
        public abstract string ViewName { get; }

        public DateTime Date { get; set; }
        public abstract ToDoType Type { get; }
    }
}