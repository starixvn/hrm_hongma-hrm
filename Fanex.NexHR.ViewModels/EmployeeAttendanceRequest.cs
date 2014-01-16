// --------------------------------------------------------------------------
// <copyright file="EmployeeAttendanceRequest.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.8.1">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// EmployeeAttendanceRequest Model.
    /// </summary>
    public class EmployeeAttendanceRequest
    {
        public RequestType RequestType { get; set; }

        public string Remark { get; set; }

        // Leave
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal? TotalDays { get; set; }

        public string LeaveType { get; set; }

        // ChangeShift
        public string FromShift { get; set; }

        public string ToShift { get; set; }

        // LateIn-EarlyOut
        public int? TotalMins { get; set; }

        // Overtime
        public OvertimeType OvertimeType { get; set; }

        public bool? IsOvertimeHours { get; set; }
    }
}
