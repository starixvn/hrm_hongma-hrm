// --------------------------------------------------------------------------
// <copyright file="LeaveType.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Leave Type.
    /// </summary>
    public class LeaveType : EntityBase<int>
    {        
        public short? IsAnnualQuota { get; set; }

        public bool? IsDefault { get; set; }

        public bool? IsSalaryDeduction { get; set; }

        public bool? IsALDeduction { get; set; }

        public bool? IsHoliday { get; set; }

        public bool IsLeaveDuringWorking { get; set; }
    }
}
