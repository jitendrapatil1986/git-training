using System.Collections.Generic;

namespace Warranty.Core.Features.EditServiceCallLineItem
{
    using System.Web.Mvc;

    public class ProblemDetailQuery : IQuery<IEnumerable<SelectListItem>>
    {
        public string ProblemJdeCode { get; set; }
    }
}
