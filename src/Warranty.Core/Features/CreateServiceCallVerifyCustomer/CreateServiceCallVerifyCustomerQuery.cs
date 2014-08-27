using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.CreateServiceCallVerifyCustomer
{
    public class CreateServiceCallVerifyCustomerQuery : IQuery<CreateServiceCallVerifyCustomerModel>
    {
        public Guid HomeOwnerId { get; set; }
    }
}
