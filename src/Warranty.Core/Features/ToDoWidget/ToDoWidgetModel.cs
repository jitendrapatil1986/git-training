using System.Collections.Generic;
using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure.Interfaces;

namespace Warranty.Core.Features.ToDoWidget
{
    public class ToDoWidgetModel
    {
        public ToDoWidgetModel()
        {
            ToDoTypes = new List<ToDoType>();
            ToDos = new List<IToDo>();
        }
        public IEnumerable<IToDo> ToDos { get; set; }
        public IEnumerable<ToDoType> ToDoTypes { get; set; }    
    }
}
