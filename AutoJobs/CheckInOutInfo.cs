// --------------------------------------------------------------------------
// <copyright file="CheckInOutInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.10.8">Created</change>
// </history>

namespace AutoJobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// CheckInOut Info.
    /// </summary>
    [Serializable]
    public class CheckInOutInfo
    {
        public string Employee { get; set; }

        public string Date { get; set; }

        public string Type { get; set; }

        public string Shift { get; set; }

        public string Remark { get; set; }
    }
}
