using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.CreateServiceCall
{
    public class CreateServiceCallQuery : IQuery<CreateServiceCallModel>
    {
        public Guid JobId { get; set; }
    }
}
