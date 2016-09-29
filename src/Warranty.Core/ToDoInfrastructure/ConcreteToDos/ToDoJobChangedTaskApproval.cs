namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    using Enumerations;
    using Models;

    public class ToDoJobChangedTaskApproval : ToDo<ToDoJobChangedTaskApprovalModel>
    {
        public override string ViewName
        {
            get { return "_ToDoJobChangedTaskApproval"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.WarrantyOrientationApproval; }
        }

        public override ToDoPriority Priority
        {
            get { return ToDoPriority.Low; }
        }
    }
}