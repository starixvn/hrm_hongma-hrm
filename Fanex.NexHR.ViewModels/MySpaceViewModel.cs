// --------------------------------------------------------------------------
// <copyright file="MySpaceViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// MySpace ViewModel.
    /// </summary>
    public class MySpaceViewModel
    {
        public decimal AnnualLeaveBalance { get; set; }

        public decimal TotalWorkHours { get; set; }

        public int TotalPendingRequest { get; set; }

        public int TotalRequest { get; set; }

        public int TotalCheckInOutRequest { get; set; }

        public int TotalPendingCheckInOutRequest { get; set; }

        public int TotalLateInEarlyOutRequest { get; set; }

        public int TotalPendingLateInEarlyOutRequest { get; set; }

        public int TotalChangedShiftRequest { get; set; }

        public int TotalPendingChangedShiftRequest { get; set; }

        public int TotalLeaveRequest { get; set; }

        public int TotalPendingLeaveRequest { get; set; }
    }
}