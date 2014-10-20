using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    public class ToDoJobStageChangedTask : ToDo<ToDoJobChangedTaskModel>
    {
        public override string ViewName
        {
            get { return "_ToDoJobChangedTask"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.JobChangedTask; }
        }

        public override ToDoPriority Priority
        {
            get { return ToDoPriority.Low; }
        }
    }
}