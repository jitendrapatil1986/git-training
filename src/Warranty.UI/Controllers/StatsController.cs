namespace Warranty.UI.Controllers
{
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Enumerations;
    using Warranty.Core.Features.ServiceCallStats;

    public class StatsController : Controller
    {
        private readonly IMediator _mediator;

        public StatsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index(string id)
        {
            var result = _mediator.Request(new ServiceCallStatsQuery{ViewId = StatView.FromDisplayNameOrDefault(id)});
            return View(result);
        }
    }
}