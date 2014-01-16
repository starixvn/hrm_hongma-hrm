// --------------------------------------------------------------------------
// <copyright file="WorkingTimeRequest.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    /// <summary>
    /// WorkingTime Request.
    /// </summary>
    public class WorkingTimeRequest
    {
        public long Id { get; set; }

        public int EmpId { get; set; }

        public short RequestType { get; set; }

        public byte Status { get; set; }

        public bool IsPlanned { get; set; }

        public string Remark { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public int? ProcessedBy { get; set; }

        public DateTime FromDate { get; set; }

        public int? FromShift { get; set; }

        public DateTime? ToDate { get; set; }

        public int? ToShift { get; set; }

        public int? LeaveTypeID { get; set; }

        public decimal? TotalDays { get; set; }

        public int? LateInMins { get; set; }

        public int? EarlyOutMins { get; set; }

        public short? OvertimeType { get; set; }

        public bool? IsOvertimeHours { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
