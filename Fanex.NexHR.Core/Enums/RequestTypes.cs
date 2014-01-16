// --------------------------------------------------------------------------
// <copyright file="RequestTypes.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum RequestTypes : short
    {
        /// <summary>
        /// CheckIn item.
        /// </summary>
        [StringValue("Check In")]
        CheckIn = 1,

        /// <summary>
        /// CheckOut item.
        /// </summary>
        [StringValue("Check Out")]
        CheckOut = 2,

        /// <summary>
        /// LateIn item.
        /// </summary>
        [StringValue("Late In")]
        LateIn = 3,

        /// <summary>
        /// EarlyOut item.
        /// </summary>
        [StringValue("Early Out")]
        EarlyOut = 4,

        /// <summary>
        /// ChangeShift item.
        /// </summary>
        [StringValue("Change Shift")]
        ChangeShift = 5,

        /// <summary>
        /// Leave item.
        /// </summary>
        [StringValue("Leave")]
        Leave = 6,

        /// <summary>
        /// Overtime item.
        /// </summary>
        [StringValue("Overtime")]
        Overtime = 7
    }
}
