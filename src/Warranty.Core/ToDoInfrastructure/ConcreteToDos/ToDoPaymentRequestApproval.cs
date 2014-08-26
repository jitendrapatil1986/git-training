using Warranty.Core.Enumerations;
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

        public override ToDoType Type
        {
            get { return ToDoType.PaymentRequestApproval; }
            set { }
        }
    }
}