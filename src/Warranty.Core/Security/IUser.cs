using System.Collections.Generic;

namespace Warranty.Core.Security
{
    public interface IUser
    {
        string UserName { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        List<string> Markets { get; }
        IEnumerable<string> Roles { get; }
        string EmployeeNumber { get; set; }
        string LoginName { get; set; }
        bool IsInRole(string role);
    }
}