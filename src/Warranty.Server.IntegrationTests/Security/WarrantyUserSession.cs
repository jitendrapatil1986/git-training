using Warranty.Core.Security;

namespace Warranty.Server.IntegrationTests.Security
{
    public class WarrantyUserSession : IUserSession
    {
        public IUser GetCurrentUser()
        {
            return new WarrantyUser("TestUser", null);
        }

        public void LogOut()
        {
            //Do nothing
        }
    }
}
