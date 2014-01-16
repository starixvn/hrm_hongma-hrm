// --------------------------------------------------------------------------
// <copyright file="ChangeWorkingCalendarsViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// ChangeWorkingCalendars ViewModel.
    /// </summary>
    public class ChangeWorkingCalendarsViewModel
    {
        public ChangeWorkingCalendarsViewModel() 
        {
            this.Date = DateTime.Now;
            this.IsOfDay = 0;
            this.ModifiedDate = DateTime.Now;
        }

        [Required]
        [Display(Name = "Date", ResourceType = typeof(Labels))]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        public int IsOfDay { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }
    }
}
