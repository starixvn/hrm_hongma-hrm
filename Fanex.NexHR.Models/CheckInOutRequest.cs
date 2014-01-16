// --------------------------------------------------------------------------
// <copyright file="CheckInOutRequest.cs" company="Nexcel Solutions Vietnam">
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
    /// CheckInOut Request.
    /// </summary>
    public class CheckInOutRequest
    {
        public long Id { get; set; }

        public short RequestType { get; set; }

        public string Shift { get; set; }

        public DateTime RequestTime { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Requester { get; set; }

        public string Team { get; set; }

        public string FullNameRequester { get; set; }

        public string ProcessedBy { get; set; }

        public string FullNameProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }

        public string Status { get; set; }
    }
}
