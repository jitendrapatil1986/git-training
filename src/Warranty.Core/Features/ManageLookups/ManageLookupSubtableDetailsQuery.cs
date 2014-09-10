using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.ManageLookups
{
    public class ManageLookupSubtableDetailsQuery : IQuery<IEnumerable<ManageLookupSubtableDetailsModel>>
    {
        public string Query { get; set; }
    }
}
