using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    public class ToDoServiceCallApproval : ToDo<ToDoServiceCallApprovalModel>
    {
        public override string ViewName
        {
            get { return "_ToDoServiceCallApproval"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.ServiceCallApproval; }
        }

        public override ToDoPriority Priority
        {
            get { return ToDoPriority.High; }
        }
    }
}