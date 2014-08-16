namespace Warranty.Tests.Core
{
    using Warranty.Core.Security;

    public class TestWarrantyUserSession : IUserSession
    {
        public IUser GetCurrentUser()
        {
            return new WarrantyUser("TestUser");
        }

        public void LogOut()
        {
            //Do nothing
        }
    }
}
