// --------------------------------------------------------------------------
// <copyright file="RequestViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Request ViewModel.
    /// </summary>
    public class RequestViewModel
    {
        public RequestViewModel()
        {
            this.StatusId = -1;
            this.FromDate = DateTime.Now.Date;
            this.ToDate = DateTime.Now.Date;
            this.DateOption = 1;
            this.TeamIds = null;
            this.EmployeeId = -1;
            this.RequestType = 6;
        }

        public int DateOption { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public string TeamIds { get; set; }

        public int RequestType { get; set; }

        [Display(Name = "Team")]
        public IList<Team> TeamCollection { get; set; }

        /// <summary>
        /// Method GetSelectListItemTeam.
        /// </summary>
        /// <returns>List SelectListItem.</returns>
        public IList<SelectListItem> GetSelecListItemTeam()
        {
            IList<SelectListItem> selecListItemTeam = new List<SelectListItem>();

            if (this.TeamCollection == null)
            {
                this.TeamCollection = new List<Team>();
            }

            var teamIds = this.TeamIds != null ? this.TeamIds.Split(',') : null;
            var isSelected = false;

            foreach (Team team in this.TeamCollection)
            {
                isSelected = false;
                if (teamIds != null)
                {
                    int pos = Array.IndexOf(teamIds, team.Id.ToString());
                    if (pos > -1)
                    {
                        isSelected = true;
                    }
                }

                var item = new SelectListItem
                {
                    Value = team.Id.ToString(),
                    Text = team.Name,
                    Selected = isSelected
                };

                selecListItemTeam.Add(item);
            }

            return selecListItemTeam;
        }
    }
}
