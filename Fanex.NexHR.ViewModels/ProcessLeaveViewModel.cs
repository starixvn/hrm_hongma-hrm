// --------------------------------------------------------------------------
// <copyright file="ProcessLeaveViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// ProcessLeave ViewModel.
    /// </summary>
    public class ProcessLeaveViewModel : BaseProcessModel
    {
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        public decimal TotalDays { get; set; }

        public string LeaveType { get; set; }

        public bool IsPlanned { get; set; }

        public string Remark { get; set; }
    }
}
