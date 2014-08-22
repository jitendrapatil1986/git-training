namespace Warranty.Core.ToDoInfrastructure.Models
{
    public class ToDoPaymentRequestApprovalModel
    {
        public string HomeOwnerName { get; set; }
        public string HomeOwnerAddress { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}