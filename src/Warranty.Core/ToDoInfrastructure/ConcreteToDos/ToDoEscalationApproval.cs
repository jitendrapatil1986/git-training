using Warranty.Core.ToDoInfrastructure.Interfaces;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    public class ToDoEscalationApproval : ToDo<ToDoEscalationApprovalModel>
    {
        public override string ViewName
        {
            get { return "_ToDoEscalationapproval"; }
            set {  }
        }
    }
}