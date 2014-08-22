using Warranty.Core.ToDoInfrastructure.Interfaces;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    public class ToDoServiceCallApproval : ToDo<ToDoServiceCallRequestApprovalModel>
    {
        public override string ViewName
        {
            get { return "_ToDoServiceCallApproval"; }
            set { }
        }
    }
}