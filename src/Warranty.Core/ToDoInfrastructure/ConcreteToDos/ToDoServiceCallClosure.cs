namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    using Enumerations;
    using Models;

    public class ToDoServiceCallClosure : ToDo<ToDoServiceCallClosureModel>
    {
        public override string ViewName
        {
            get { return "_ToDoServiceCallClosure"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.ServiceCallClosure; }
        }

        public override ToDoPriority Priority
        {
            get { return ToDoPriority.High; }
        }
    }
}