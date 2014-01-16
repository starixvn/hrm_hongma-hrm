// --------------------------------------------------------------------------
// <copyright file="EmployeeAttendanceOvertime.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.16">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// EmployeeAttendanceOvertime Model.
    /// </summary>
    public class EmployeeAttendanceOvertime
    {
        public string OvertimeName { get; set; }

        public OvertimeType OvertimeType { get; set; }

        public DateTime? OvertimeFromTime { get; set; }

        public DateTime? OvertimeToTime { get; set; }

        public decimal? OvertimeTotalHours { get; set; }
    }
}
