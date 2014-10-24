using System.Collections.Generic;

namespace Warranty.Core.Features.EditServiceCallLineItem
{
    using System.Web.Mvc;

    public class RootCauseQuery : IQuery<IEnumerable<SelectListItem>>
    {
        public string ProblemCode { get; set; }
    }
}
