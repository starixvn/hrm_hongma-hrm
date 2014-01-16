// --------------------------------------------------------------------------
// <copyright file="TotalWorkingHours.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Total WorkingHours.
    /// </summary>
    public class TotalWorkingHours
    {
        public Employee Employee { get; set; }

        public int WorkingDay { get; set; }

        public float ActualWorkingDay { get; set; }

        public float LeaveTaken { get; set; }

        public float UnpaidLeave { get; set; }

        public int LateInMins { get; set; }

        public int EarlyOutMins { get; set; }

        public float TotalHours { get; set; }

        public float AverageHours { get; set; }
    }
}
