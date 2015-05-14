namespace Warranty.Tests.Core
{
    using Common.Security.User.Session;
    using Common.Security.User;

    public class TestWarrantyUserSession : IUserSession
    {
        public IUser GetCurrentUser()
        {
            return new UserInfo("TestUser");
        }

        public void LogOut()
        {
            //Do nothing
        }
    }
}
