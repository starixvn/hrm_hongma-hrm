// --------------------------------------------------------------------------
// <copyright file="RequestEntity.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Models.Request
{
    using System;

    /// <summary>
    /// Request Entity.
    /// </summary>
    public class RequestEntity : EntityBase<long>
    {
        public Employee Requester { get; set; }

        public DateTime RequestedDate { get; set; }

        public RequestType Type { get; set; }

        public RequestStatus Status { get; set; }        

        public Employee CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsPlanned { get; set; }

        public string Remark { get; set; }

        public decimal TotalDaysRequestInMonth { get; set; }

        // Leave
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal TotalDays { get; set; }

        public string LeaveType { get; set; }

        // ChangeShift
        public Shift FromShift { get; set; }

        public Shift ToShift { get; set; }

        // LateIn-EarlyOut
        public int TotalMins { get; set; }

        // Overtime
        public short OvertimeType { get; set; }

        public bool IsOvertimeHours { get; set; }
    }
}
