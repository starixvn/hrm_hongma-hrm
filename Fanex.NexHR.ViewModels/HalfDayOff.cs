// --------------------------------------------------------------------------
// <copyright file="HalfDayOff.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// HalfDayOff Model.
    /// </summary>
    public class HalfDayOff
    {
        public int ID { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int Day { get; set; }

        public string Code { get; set; }
    }
}
