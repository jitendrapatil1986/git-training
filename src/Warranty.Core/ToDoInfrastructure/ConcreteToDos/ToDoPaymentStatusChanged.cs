namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    using Enumerations;
    using Models;

    public class ToDoPaymentStatusChanged : ToDo<ToDoPaymentStatusChangedModel>
    {
        public override string ViewName
        {
            get { return "_ToDoPaymentStatusChanged"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.PaymentStatusChanged; }
        }

        public override ToDoPriority Priority
        {
            get { return ToDoPriority.High; }
        }
    }
}