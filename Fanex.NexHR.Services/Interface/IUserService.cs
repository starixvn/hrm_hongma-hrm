namespace Fanex.NexHR.Services.Interface
{
    public interface IUserService
    {
        UserSession CurrentUser { get; }
    }
}
