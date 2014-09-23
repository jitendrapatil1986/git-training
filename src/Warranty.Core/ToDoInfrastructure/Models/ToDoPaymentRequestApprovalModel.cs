namespace Warranty.Core.ToDoInfrastructure.Models
{
    public class ToDoPaymentRequestApprovalModel
    {
        //TODO: Not the final model
        public string HomeOwnerName { get; set; }
        public string HomeOwnerAddress { get; set; }
        public decimal PaymentAmount { get; set; }
        public int HomeOwnerNumber { get; set; }
    }
}