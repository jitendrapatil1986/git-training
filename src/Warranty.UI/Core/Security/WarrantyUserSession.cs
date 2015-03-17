namespace Warranty.UI.Core.Security
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;
    using Microsoft.IdentityModel.Web;
    using Warranty.Core.Security;

    public class WarrantyUserSession : IUserSession
    {
        private const string SecMarketPrefix = "SEC_Market_";
        private const string OrgMarketPrefix = "ORG_Market_";

        public IUser GetCurrentUser()
        {
            var userName = HttpContext.Current.User.Identity.Name;

            return new WarrantyUser(userName, GetAllRoles())
            {
                FirstName = GetClaimsValueOrDefault(Microsoft.IdentityModel.Claims.ClaimTypes.GivenName),
                LastName = GetClaimsValueOrDefault(Microsoft.IdentityModel.Claims.ClaimTypes.Surname),
                Email = GetClaimsValueOrDefault(Microsoft.IdentityModel.Claims.ClaimTypes.Email),
                EmployeeNumber = GetClaimsValueOrDefault(Microsoft.IdentityModel.Claims.ClaimTypes.PPID),
                LoginName = GetClaimsValueOrDefault(Microsoft.IdentityModel.Claims.ClaimTypes.WindowsAccountName),
                Markets = GetMarkets(GetAllRoles())
            };
        }

        public void LogOut()
        {
            var sessionModule = FederatedAuthentication.SessionAuthenticationModule;
            if (sessionModule != null)
                sessionModule.SignOut();
        }

        private static IEnumerable<string> GetAllRoles()
        {
            var roles = new List<string>();
            var claimsPrincipal = HttpContext.Current.User as ClaimsPrincipal;
            if (claimsPrincipal != null)
            {
                var roleClaims = claimsPrincipal.FindAll(ClaimTypes.Role);

                roles.AddRange(roleClaims.Select(roleClaim => roleClaim.Value));
            }

            return roles;
        }

        private static string GetClaimsValueOrDefault(string claimType)
        {
            var claimValue = "N/A";
            var claimsPrincipal = HttpContext.Current.User as ClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                var claim = claimsPrincipal.FindFirst(claimType);
                if (claim != null && claim.Value != null)
                {
                    claimValue = claim.Value;
                }
            }

            return claimValue;
        }

        private static List<string> GetMarkets(IEnumerable<string> roles)
        {
            var markets = roles.Where(x => x.StartsWith(OrgMarketPrefix) || x.StartsWith(SecMarketPrefix));

            return markets.Select(x => x.Replace(OrgMarketPrefix, "").Replace(SecMarketPrefix, "")).ToList();
        }
    }
}