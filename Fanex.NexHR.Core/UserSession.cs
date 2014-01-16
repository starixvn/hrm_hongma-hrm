// --------------------------------------------------------------------------
// <copyright file="UserSession.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System;

    /// <summary>
    /// User Session.
    /// </summary>
    [Serializable]
    public class UserSession 
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public short Role { get; set; }
    }
}
