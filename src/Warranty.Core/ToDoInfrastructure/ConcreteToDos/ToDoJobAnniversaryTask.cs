namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    using Enumerations;
    using Models;

    public class ToDoJobAnniversaryTask : ToDo<ToDoJobAnniversaryTaskModel>
    {
        public override string ViewName
        {
            get { return "_ToDoJobAnniversaryTask"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.JobAnniversaryTask; }
        }

        public override ToDoPriority Priority
        {
            get { return ToDoPriority.Low; }
        }
    }
}