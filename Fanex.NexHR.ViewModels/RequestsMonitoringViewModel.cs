// --------------------------------------------------------------------------
// <copyright file="RequestsMonitoringViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// RequestMonitoring ViewModel.
    /// </summary>
    public class RequestsMonitoringViewModel
    {
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Total Request >=")]
        public int MaxRequest { get; set; }

        public int DateOption { get; set; }
    }
}
