namespace Fanex.NexHR.Services
{
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Repositories;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Repositories;

    public class UserService : IUserService
    {
        private IEmployeeRepository _employeeRepository;

        public UserService()
        {
            _employeeRepository = new EmployeeRepository();
        }

        public UserService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public UserSession CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["UserSession"] != null)
                {
                    return HttpContext.Current.Session["UserSession"] as UserSession;
                }

                var user = _employeeRepository.Get(HttpContext.Current.User.Identity.Name);
                var userSession = new UserSession
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    NickName = user.NickName,
                    Role = user.Role
                };
                HttpContext.Current.Session["UserSession"] = userSession;
                return userSession;
            }
        }
    }
}
