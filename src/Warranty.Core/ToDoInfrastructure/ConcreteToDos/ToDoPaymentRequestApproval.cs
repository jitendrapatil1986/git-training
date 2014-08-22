using Warranty.Core.ToDoInfrastructure.Interfaces;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    public class ToDoPaymentRequestApproval : ToDo<ToDoPaymentRequestApprovalModel>
    {
        public override string ViewName
        {
            get { return "_ToDoPaymentRequestApproval"; }
            set { }
        }
    }
}