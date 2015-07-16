namespace Warranty.Tests.Core
{
    using Common.Security.Session;
    using Common.Security.User;

    public class TestWarrantyUserSession : IUserSession
    {
        public IUser GetCurrentUser()
        {
            return new UserInfo("TestUser");
        }

        public IUser GetActualUser()
        {
            return GetCurrentUser();
        }

        public void LogOut()
        {
            //Do nothing
        }
    }
}
