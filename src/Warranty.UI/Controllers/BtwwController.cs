using System.IO;
using System.Web;
using System.Web.Mvc;
using Warranty.Core;
using Warranty.Core.Features.BuiltTheWeekleyWay;
using Warranty.Core.Services;

namespace Warranty.UI.Controllers
{ 
    public class BtwwController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IBuiltTheWeekleyWayService _builtTheWeekleyWayService;

        public BtwwController(IMediator mediator, IBuiltTheWeekleyWayService builtTheWeekleyWayService)
        {
            _mediator = mediator;
            _builtTheWeekleyWayService = builtTheWeekleyWayService;
        }

        public ActionResult Index()
        {
            var model = _mediator.Request(new BuiltTheWeekleyWayQuery());
            return View(model);
        }

        public FileResult DownloadDocument(string fileName, string marketCode)
        {
            var marketPath = _builtTheWeekleyWayService.GetMarketPath(marketCode);
            var pathToFile = Path.Combine(marketPath, fileName);
            var fileBytes = System.IO.File.ReadAllBytes(pathToFile);

            return File(fileBytes, MimeMapping.GetMimeMapping(fileName), fileName);
        }

    }
}
