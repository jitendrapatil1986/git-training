namespace Warranty.Core.ToDoInfrastructure.Models
{
    public class ToDoPaymentRequestApprovalModel
    {
        //Not the final model
        public string HomeOwnerName { get; set; }
        public string HomeOwnerAddress { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}