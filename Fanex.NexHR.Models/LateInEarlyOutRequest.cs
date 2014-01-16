// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutRequest.cs" company="Nexcel Solutions Vietnam">
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
    /// LateInEarlyOut Request.
    /// </summary>
    public class LateInEarlyOutRequest
    {
        public int Id { get; set; }

        public DateTime FromDate { get; set; }

        public int LateInMins { get; set; }

        public int EarlyOutMins { get; set; }

        public DateTime CreatedDate { get; set; }

        public byte Status { get; set; }

        public string ProcessedBy { get; set; }

        public string FullNameProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }
    }
}
