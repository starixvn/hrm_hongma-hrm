// --------------------------------------------------------------------------
// <copyright file="RolesEnum.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum RolesEnum : short
    {
        /// <summary>
        /// Member item.
        /// </summary>
        [StringValue("Member")]
        Member = 0,

        /// <summary>
        /// AssistantLeader item.
        /// </summary>
        [StringValue("AssistantLeader")]
        AssistantLeader = 1,

        /// <summary>
        /// Leader item.
        /// </summary>
        [StringValue("Leader")]
        Leader = 2,

        /// <summary>
        /// HR enum item.
        /// </summary>
        [StringValue("HR")]
        HR = 3,

        /// <summary>
        /// SA enum item.
        /// </summary>
        [StringValue("SA")]
        SA = 4
    }
}