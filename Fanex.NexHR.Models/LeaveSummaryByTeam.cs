// --------------------------------------------------------------------------
// <copyright file="LeaveSummaryByTeam.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Leave Summary group By Team.
    /// </summary>
    public class LeaveSummaryByTeam
    {
        public Team Team { get; set; }

        public List<LeaveSummary> LeaveSummaryInTeam { get; set; }
    }
}
