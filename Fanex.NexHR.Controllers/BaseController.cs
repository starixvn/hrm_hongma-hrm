// --------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Controllers
{
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Models;

    /// <summary>
    /// Base Controller.
    /// </summary>
    [Authorize]
    public class BaseController : Controller
    {
        public Employee CurrentUser
        {
            get
            {
                return (Employee)HttpContext.Session["CurrentUser"];
            }
        }

        /// <summary>
        /// On Action Executing.
        /// </summary>
        /// <param name="filterContext">Filter Context.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ////authorize user login
            if (HttpContext.Session["CurrentUser"] == null ||
                    !HttpContext.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();

                if (!Request.IsAjaxRequest())
                {
                    string url = string.Empty;
                    var uri = Request.UrlReferrer ?? Request.Url;
                    if (uri != null && uri.Host == Request.Url.Host)
                    {
                        url = string.Format("/?url={0}", Url.Encode(uri.ToString()));
                    }

                    filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                }
                else
                {
                    string lostLogin = "Session is expired. Please press F5 to login again.";
                    var contentType = filterContext.HttpContext.Request.ContentType ?? "text/html";
                    if (contentType.Contains("json"))
                    {
                        var result = new JsonResult();
                        result.Data = new { Code = -101, Message = lostLogin };
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        filterContext.Result = result;
                    }
                    else
                    {
                        filterContext.Result = Content(lostLogin);
                    }
                }
            }
        }

        /// <summary>
        /// On Authorization.
        /// </summary>
        /// <param name="filterContext">Filter Context.</param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            Employee currentUser = this.CurrentUser;

            var attributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            if (attributes.Length == 0)
            {
                attributes = GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);
            }

            if (attributes.Length == 0)
            {
                return;
            }

            if (currentUser != null)
            {
                foreach (AuthorizeAttribute item in attributes)
                {
                    if (item.Roles != string.Empty && !currentUser.IsInRoles(item.Roles.Split(new[] { ',' }).ToList()))
                    {
                        filterContext.Result = new RedirectResult("~/home/accessdenied?url=" + Request.Url.AbsoluteUri);
                    }
                }
            }
        }
    }
}
