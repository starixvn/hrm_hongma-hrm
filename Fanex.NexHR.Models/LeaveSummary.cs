// --------------------------------------------------------------------------
// <copyright file="LeaveSummary.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>
    /// Leave Summary.
    /// </summary>
    public class LeaveSummary
    {
        public Employee Employee { get; set; }

        public float AnnualLeaveLastYear { get; set; }

        public float AnnualLeaveCurrentYear { get; set; }

        public float Jan { get; set; }

        public float Feb { get; set; }  
      
        public float Mar { get; set; }

        public float Apr { get; set; }

        public float May { get; set; }

        public float Jun { get; set; }

        public float Jul { get; set; }

        public float Aug { get; set; }

        public float Sep { get; set; }

        public float Oct { get; set; }

        public float Nov { get; set; } 
       
        public float Dec { get; set; }

        public float LeaveBalance { get; set; }

        public string Remark { get; set; }
    }
}
