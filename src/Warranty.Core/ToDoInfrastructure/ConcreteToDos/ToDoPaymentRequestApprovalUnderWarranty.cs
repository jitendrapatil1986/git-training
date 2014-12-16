using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    public class ToDoPaymentRequestApprovalUnderWarranty : ToDo<ToDoPaymentRequestApprovalUnderWarrantyModel>
    {
        public override string ViewName
        {
            get { return "_ToDoPaymentRequestApprovalUnderWarranty"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.PaymentRequestApprovalUnderWarranty; }
        }
    }
}