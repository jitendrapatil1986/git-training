using System;

namespace Warranty.Core.Features.CreateServiceCallVerifyCustomer
{
    public class CreateServiceCallVerifyCustomerQuery : IQuery<CreateServiceCallVerifyCustomerModel>
    {
        public Guid HomeOwnerId { get; set; }
    }
}
