// --------------------------------------------------------------------------
// <copyright file="Combres.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

[assembly: WebActivator.PreApplicationStartMethod(typeof(Fanex.NexHR.Site.App_Start.Combres), "PreStart")]

namespace Fanex.NexHR.Site.App_Start
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Routing;
    using global::Combres;
    
    /// <summary>
    /// COMBRES class.
    /// </summary>
    public static class Combres
    {
        public static void PreStart()
        {
            RouteTable.Routes.AddCombresRoute("Combres");
        }
    }
}