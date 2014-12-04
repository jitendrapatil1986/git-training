namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    using Enumerations;
    using Models;

    public class ToDoPaymentRequestApprovalOutOfWarranty : ToDo<ToDoPaymentRequestApprovalOutOfWarrantyModel>
    {
        public override string ViewName
        {
            get { return "_ToDoPaymentRequestApprovalOutOfWarranty"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.PaymentRequestApprovalOutOfWarranty; }
        }
    }
}