namespace Warranty.Core.Features.SelectListProviders
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public interface ISelectListProvider
    {
        IEnumerable<SelectListItem> Provide();
    }
}
