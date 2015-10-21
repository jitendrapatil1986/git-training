namespace Warranty.Server.Security
{
    using Common.Security.User;
    using Common.Security.Session;

    public class WarrantyServerUserSession : IUserSession
    {
        public IUser GetCurrentUser()
        {
            return new UserInfo("Warranty.Server") { LoginName = "WARRANTY" };
        }

        public IUser GetActualUser()
        {
            return GetCurrentUser();
        }

        public void LogOut()
        {
        }
    }
}
