namespace Fanex.NexHR.Core.Services
{
    public interface IUserService
    {
        UserSession CurrentUser { get; }
    }
}
