// --------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
//   <change who="Anders.Le" date="2012.8.27">Add application error</change>
// </history>
namespace Fanex.NexHR.Site
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.NexHR.Web.Providers;

    /// <summary>
    /// MVC Application.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // filters.Add(new System.Web.Http.AuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());             
        }

        /// <summary>
        /// Register Routes.
        /// </summary>
        /// <param name="routes">Route Collection.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = System.Web.Http.RouteParameter.Optional });

            routes.MapRoute(
               name: "ManageTeam",
               url: "Employee/AddTeam/{teamId}",
               defaults: new { controller = "employee", action = "AddTeam", teamId = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "employee", action = "myspace", id = UrlParameter.Optional });
        }

        public static void RegisterCustomModelBinder()
        {
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();
            ModelMetadataProviders.Current = new FriendlyNameModelMetadataProvider();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            /*if (Context.Response.StatusCode == 401)
            {
                Response.Redirect("~/home/accessdenied?url=" + Request.Url.AbsoluteUri);
            }*/
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        private void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            if (ex is HttpException)
            {
                return;
            }

            Fanex.Logging.LogService.Error(ex.Message, ex);
        }
    }
}