// --------------------------------------------------------------------------
// <copyright file="RequestFilter.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.24">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum RequestFilter : int
    {
        /// <summary>
        /// Leave item.
        /// </summary>
        [StringValue("Leave")]
        Leave = 1,

        /// <summary>
        /// ChangeShift item.
        /// </summary>
        [StringValue("Change Shift")]
        ChangeShift = 2,

        /// <summary>
        /// LateInEarlyOut item.
        /// </summary>
        [StringValue("Late In/Early Out")]
        LateInEarlyOut = 3,

        /// <summary>
        /// CheckInOut item.
        /// </summary>
        [StringValue("Check In/Check Out")]
        CheckInOut = 4,

        /// <summary>
        /// Overtime item.
        /// </summary>
        [StringValue("Overtime")]
        Overtime = 5,

        /// <summary>
        /// All value item.
        /// </summary>
        [StringValue("All")]
        All = 6
    }
}
