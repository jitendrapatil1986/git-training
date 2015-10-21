using System.Web.Mvc;
using Common.Security.Session;

namespace Warranty.UI.Controllers
{

    public class HelpController : Controller
    {
        private readonly IUserSession _userSession;

        public HelpController(IUserSession userSession)
        {
            _userSession = userSession;
        }

        public ActionResult UserDetails()
        {
            var user = _userSession.GetCurrentUser();
            return View(user);
        }

        public ActionResult SignOut(string redirect)
        {
            _userSession.LogOut();
            if (!string.IsNullOrWhiteSpace(redirect))
                return Redirect(redirect);
            return RedirectToAction("Index", "Home");
        }

    }
}
