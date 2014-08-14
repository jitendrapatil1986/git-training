using System.Collections.Generic;
using System.Linq;
using Warranty.Core.Security;

namespace Warranty.Server.Security
{
    public class WarrantyServerUser : IUser
    {
        public WarrantyServerUser()
        {
            UserName = "Warranty.Server";
        }

        public string UserName { get; private set; }
        public IEnumerable<string> Roles { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> Markets { get; set; }
        public string EmployeeNumber { get; set; }
        public string LoginName { get; set; }

        public bool IsInRole(string role)
        {
            if (Roles == null)
                return false;

            return Roles.Any() && Roles.Contains(role);
        }
    }
}