// --------------------------------------------------------------------------
// <copyright file="MissingCheckInOutModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// MissingCheckInOut Model.
    /// </summary>
    public class MissingCheckInOutModel
    {
        public long Id { get; set; }

        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string Team { get; set; }

        public string TeamCode { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string ShiftCode { get; set; }

        public string Email { get; set; }

        public int Type { get; set; }
    }
}
