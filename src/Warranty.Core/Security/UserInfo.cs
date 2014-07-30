using System.Collections.Generic;
using System.Linq;

namespace Warranty.Core.Security
{
    public class UserInfo : IUser
    {
        public UserInfo(string userName, IEnumerable<string> roles = null)
        {
            UserName = userName;
            Roles = roles;
        }

        public string UserName { get; private set; }
        public IEnumerable<string> Roles { get; private set; }
        public string EmployeeNumber { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> Markets { get; set; }
        public string LocalId { get; set; }
        public string LoginName { get; set; }
        
        public bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }
    }
}