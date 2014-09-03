namespace Warranty.Core.Security
{
    using System.Collections.Generic;
    using System.Linq;

    public class WarrantyUser : IUser
    {
        public WarrantyUser(string userName, IEnumerable<string> roles = null)
        {
            UserName = userName;
            Roles = roles;
        }

        public string UserName { get; private set; }
        public IEnumerable<string> Roles { get; private set; }

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