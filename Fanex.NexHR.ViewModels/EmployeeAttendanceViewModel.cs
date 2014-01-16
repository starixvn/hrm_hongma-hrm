// --------------------------------------------------------------------------
// <copyright file="EmployeeAttendanceViewModel.cs" company="Nexcel Solutions Vietnam">
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
    /// EmployeeAttendance ViewModel.
    /// </summary>
    public class EmployeeAttendanceViewModel
    {
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Employee")]
        public int? EmpId { get; set; }

        public string Employee { get; set; }

        public string GroupIds { get; set; }

        public short Type { get; set; }

        public int DateOption { get; set; }

        public bool ExcludeUserDelete { get; set; }
    }
}
