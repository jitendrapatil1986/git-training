using System;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Warranty.Core.Services
{
    public class ToDoFilterCookieManager : IManageToDoFilterCookie
    {
        private const string CookieName = "todo_last_selected_filter";
        private const string Purpose = "ToDo Filter Management";

        public void Write(string value)
        {
            var cookie = new HttpCookie(CookieName);
            var encodedValue = MachineKey.Protect(Encoding.UTF8.GetBytes(value ?? string.Empty), Purpose);
            cookie.Value = HttpServerUtility.UrlTokenEncode(encodedValue);

            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        public string Read()
        {
            var cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null)
            {
                var stream = HttpServerUtility.UrlTokenDecode(cookie.Value);
                var decodedValue = MachineKey.Unprotect(stream, Purpose);
                return Encoding.UTF8.GetString(decodedValue);
            }
            return null;
        }
    }
}