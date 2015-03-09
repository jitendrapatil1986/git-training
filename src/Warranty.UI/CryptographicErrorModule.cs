using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Warranty.UI
{
    /// <summary>
    /// This module will fire ContextOnError when there is an exception in the application.  It does nothing unless the exception 
    /// was a CryptographicException.  If a CryptographicException, it will delete all existing cookies.  Once the user reloads, they 
    /// will re-authenticate, hopefully addressing the underlying issue of the cryptographic exception.
    /// See cc-3659 in youtrack.
    /// Solution derived from http://stackoverflow.com/a/28858597/17803
    /// </summary>
    public class CryptographicErrorModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.Error += ContextOnError;
        }

        private void ContextOnError(object sender, EventArgs eventArgs)
        {
            var context = HttpContext.Current;
            if (context == null)
                return;

            var error = context.Server.GetLastError();
            var cryptoError = error as CryptographicException;
            if (cryptoError == null)
                return;

            if (context.Request.Cookies["CryptoErrorOccured"] != null)
                return;

            context.Response.Cookies.Clear();
            var cookieCount = context.Request.Cookies.Count;
            for (int i = 0; i < cookieCount; ++i)
            {
                var httpCookie = context.Request.Cookies[i];
                if (httpCookie != null)
                {
                    var cookieKey = httpCookie.Name;

                    var cookie = new HttpCookie(cookieKey)
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        Value = "",
                        Path = httpCookie.Path,
                        Domain = httpCookie.Domain,
                        Secure = httpCookie.Secure,
                        HttpOnly = httpCookie.HttpOnly
                    };

                    context.Response.Cookies.Add(cookie);
                }
            }

            var cryptoErrorCookie = new HttpCookie("CryptoErrorOccured", DateTime.UtcNow.ToString("G"))
            {
                Expires = DateTime.Now.AddMinutes(5)
            };

            context.Response.Cookies.Add(cryptoErrorCookie);
            context.Server.ClearError();
        }

        public void Dispose()
        {
        }
    }
}