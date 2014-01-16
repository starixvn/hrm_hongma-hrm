// --------------------------------------------------------------------------
// <copyright file="EmployeeAttendanceModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// EmployeeAttendance Model.
    /// </summary>
    public class EmployeeAttendanceModel
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string Team { get; set; }

        public string TeamCode { get; set; }

        public DateTime? AttendanceDate { get; set; }

        public int? ShiftId { get; set; }

        public string ShiftCode { get; set; }

        public string ExpectedInTime { get; set; }

        public string ExpectedOutTime { get; set; }

        public DateTime? InFixed { get; set; }

        public DateTime? OutFixed { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public decimal TotalWorkHours { get; set; }

        public bool Absence { get; set; }

        public int LeaveTypeId { get; set; }

        public bool IsRightShift { get; set; }

        public string Notes { get; set; }

        public string Remark { get; set; }

        public int IsLateIn { get; set; }

        public int IsEarlyOut { get; set; }

        public int IsMissingCheckin { get; set; }

        public int IsMissingCheckOut { get; set; }

        public int IsNotWork { get; set; }

        public List<EmployeeAttendanceRequest> RequestList { get; set; }

        public List<EmployeeAttendanceOvertime> OvertimeList { get; set; }
    }
}
