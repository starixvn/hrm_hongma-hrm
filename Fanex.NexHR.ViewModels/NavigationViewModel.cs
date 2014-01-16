// --------------------------------------------------------------------------
// <copyright file="NavigationViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    /// <summary>
    /// Navigation ViewModel.
    /// </summary>
    public class NavigationViewModel
    {
        private string _controller;
        private string _action;
        private string _title;

        public NavigationViewModel()
        {
        }

        public NavigationViewModel(SiteMapNode sitemapNode)
        {
            if (sitemapNode != null)
            {
                this.Controller = sitemapNode["Controller"];
                this.Action = sitemapNode["Action"];
                this.Title = sitemapNode.Title;
                this.Icon = sitemapNode["Icon"];
                this.HoverController = sitemapNode["HoverController"];
                this.HoverAction = sitemapNode["HoverAction"];
                this.HoverIcon = sitemapNode["HoverIcon"];
            }
        }

        public string Icon { get; set; }

        public string Controller
        {
            get
            {
                return this._controller.ToLower();
            }

            set
            {
                this._controller = value ?? string.Empty;
            }
        }

        public string Action
        {
            get
            {
                return this._action.ToLower();
            }

            set
            {
                this._action = value ?? string.Empty;
            }
        }

        public string Title
        {
            get
            {
                return this._title;
            }

            set
            {
                this._title = value ?? string.Empty;
            }
        }

        public IDictionary<string, object> HtmlAttributes { get; set; }

        public List<NavigationViewModel> Childs { get; set; }

        public string Id
        {
            get
            {
                return string.Format("{0}_{1}", this.Controller, this.Action);
            }
        }

        public string HoverController { get; set; }

        public string HoverAction { get; set; }

        public string HoverIcon { get; set; }
    }
}
