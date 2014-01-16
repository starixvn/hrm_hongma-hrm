// --------------------------------------------------------------------------
// <copyright file="TotalWorkingHoursViewModel.cs" company="Nexcel Solutions Vietnam">
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

    /// <summary>
    /// TotalWorkingHours ViewModel.
    /// </summary>
    public class TotalWorkingHoursViewModel
    {
        [DataType(DataType.DateTime)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ToDate { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int DateOption { get; set; }

        public bool IsExportExcel { get; set; }
    }
}
