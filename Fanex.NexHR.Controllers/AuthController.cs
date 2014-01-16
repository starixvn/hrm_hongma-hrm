// --------------------------------------------------------------------------
// <copyright file="AuthController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// AUTH Controller.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public AuthController()
        {
            _employeeService = new EmployeeService();
        }

        public AuthController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login Action.
        /// </summary>
        /// <param name="model">LoginModel Object.</param>
        /// <returns>Action Result Login.</returns>
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                int retCode = _employeeService.ValidateUser(model.UserName, model.Password);
                switch (retCode)
                {
                    case 0: // OK
                        HttpContext.Session["CurrentUser"] = _employeeService.Get(model.UserName);
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        return RedirectToAction("Index", "Home");

                    case 1: // deleted
                        ModelState.AddModelError(string.Empty, Resources.Messages.DeletedUser);
                        break;

                    case 2: // disalbed
                        ModelState.AddModelError(string.Empty, Resources.Messages.DisabledUser);
                        break;

                    case -1:
                    case 3:
                    default: // wrong username/password
                        ModelState.AddModelError(string.Empty, Resources.Messages.WrongUsernameOrPassword);
                        break;
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (HttpContext.Session != null)
            {
                HttpContext.Session.Abandon();
                HttpContext.Session.RemoveAll();
            }

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
