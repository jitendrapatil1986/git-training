namespace Warranty.Server.Security
{
    using Core.Security;

    public class WarrantyServerUserSession : IUserSession
    {
        public IUser GetCurrentUser()
        {
            return new WarrantyUser("Warranty.Server") {LoginName = "WARRANTY"};
        }

        public void LogOut()
        {
        }
    }
}
