using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    public class ToDoEscalationApproval : ToDo<ToDoEscalationApprovalModel>
    {
        public override string ViewName
        {
            get { return "_ToDoEscalationApproval"; }
        }

        public override ToDoType Type {
            get { return ToDoType.EscalationApproval; }
        }
    }
}