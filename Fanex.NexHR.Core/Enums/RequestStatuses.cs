// --------------------------------------------------------------------------
// <copyright file="RequestStatuses.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum RequestStatuses : byte
    {
        /// <summary>
        /// Pending item.
        /// </summary>
        [StringValue("Pending")]
        Pending = 1,

        /// <summary>
        /// Approve item.
        /// </summary>
        [StringValue("Approved")]
        Approve = 2,

        /// <summary>
        /// Reject item.
        /// </summary>
        [StringValue("Rejected")]
        Reject = 3
    }
}
