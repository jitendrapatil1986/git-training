using System.Collections.Generic;

public interface IUser
{
    string UserName { get; }
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    List<string> Markets { get; }
    IEnumerable<string> Roles { get; }
    string LocalId { get; set; }
    string LoginName { get; set; }
}