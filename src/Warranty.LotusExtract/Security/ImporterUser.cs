using System.Collections.Generic;
using Warranty.Core.Security;

namespace Warranty.LotusExtract.Security
{
    public class ImporterUser : IUser
    {
        public ImporterUser()
        {
            UserName = "LotusImporter";
            LoginName = "LotusImporter";
            FirstName = "Lotus";
            LastName = "Importer";
        }

        public string UserName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public List<string> Markets { get; private set; }
        public IEnumerable<string> Roles { get; private set; }
        public string EmployeeNumber { get; set; }
        public string LoginName { get; set; }
        
        public bool IsInRole(string role)
        {
            throw new System.NotSupportedException();
        }
    }
}