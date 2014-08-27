using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.CreateServiceCallCustomerSearch
{
    public class CreateServiceCallCustomerSearchQuery : IQuery<CreateServiceCallCustomerSearchModel>
    {
        public string SearchCriteria { get; set; }
    }
}
