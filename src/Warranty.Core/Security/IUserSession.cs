namespace Warranty.Core.Security
{
    public interface IUserSession
    {
        IUser GetCurrentUser();

        void LogOut();
    }
}