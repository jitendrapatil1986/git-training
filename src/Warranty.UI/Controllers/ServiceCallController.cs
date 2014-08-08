namespace Warranty.UI.Controllers
{
    using System;
    using System.Web.Mvc;

    public class ServiceCallController : Controller
    {
         public ActionResult Reassign(Guid id)
         {
             return View();
         }

         public ActionResult AddNote(Guid id)
         {
             return View();
         }

         public ActionResult Close(Guid id)
         {
             return View();
         }

        public ActionResult RequestPayment(Guid id)
        {
            return View();
        }
    }
}