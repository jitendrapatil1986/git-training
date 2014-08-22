namespace Warranty.Core.Enumerations
{
    public class ToDoType : Enumeration<ToDoType>
    {
        public static readonly ToDoType ServiceCallApproval = new ToDoType(1, "Service Call Approval");
        public static readonly ToDoType EscalationApproval = new ToDoType(2, "Escalation Approval");
        public static readonly ToDoType PaymentRequestApproval = new ToDoType(3, "Payment Request Approval");

        private ToDoType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }

    public class ToDoIcon : Enumeration<ToDoIcon>
    {
        public static readonly ToDoIcon ServiceCallApproval = new ToDoIcon(1, "Service Call Approval");
        public static readonly ToDoIcon EscalationApproval = new ToDoIcon(2, "Escalation Approval");
        public static readonly ToDoIcon PaymentRequestApproval = new ToDoIcon(3, "Payment Request Approval");

        private ToDoIcon(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
