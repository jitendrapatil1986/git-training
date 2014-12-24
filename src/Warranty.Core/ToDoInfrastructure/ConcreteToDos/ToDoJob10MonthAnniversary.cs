namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    using Enumerations;
    using Models;

    public class ToDoJob10MonthAnniversary : ToDo<ToDoJob10MonthAnniversaryModel>
    {
        public override string ViewName
        {
            get { return "_ToDoJob10MonthAnniversary"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.JobAnniversaryTask; }
        }

        public override ToDoPriority Priority
        {
            get { return ToDoPriority.High; }
        }
    }
}