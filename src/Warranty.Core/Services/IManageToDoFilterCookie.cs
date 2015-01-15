namespace Warranty.Core.Services
{
    public interface IManageToDoFilterCookie
    {
        void Write(string value);
        string Read();
    }
}