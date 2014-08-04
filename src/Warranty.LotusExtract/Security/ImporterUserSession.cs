using Warranty.Core.Security;

namespace Warranty.LotusExtract.Security
{
    public class ImporterUserSession : IUserSession
    {
        public IUser GetCurrentUser()
        {
            return new ImporterUser();
        }

        public void LogOut()
        {
            
        }
    }
}