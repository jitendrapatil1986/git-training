using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Warranty.Core.Security
{
    public class UserSession : IUserSession
    {
        private const string MarketOrgPrefix = "ORG_Market_";

        public IUser GetCurrentUser()
        {
            var userName = HttpContext.Current.User.Identity.Name;

            return new UserInfo(userName, GetAllRoles())
            {
                FirstName = GetClaimsValueOrDefault(Microsoft.IdentityModel.Claims.ClaimTypes.GivenName),
                LastName = GetClaimsValueOrDefault(Microsoft.IdentityModel.Claims.ClaimTypes.Surname),
                Email = GetClaimsValueOrDefault(Microsoft.IdentityModel.Claims.ClaimTypes.Email),
                LocalId = GetClaimsValueOrDefault(Microsoft.IdentityModel.Claims.ClaimTypes.PPID),
                Markets = GetMarkets(GetAllRoles())
            };
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
            var markets = roles.Where(x => x.StartsWith(MarketOrgPrefix));

            return markets.Select(x => x.Replace(MarketOrgPrefix, "")).ToList();
        }
    }
}