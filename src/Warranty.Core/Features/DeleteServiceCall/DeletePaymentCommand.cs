namespace Warranty.Core.Features.DeleteServiceCall
{
    using System;

    public class DeletePaymentCommand : ICommand
    {
        public Guid PaymentId { get; set; }
    }
}