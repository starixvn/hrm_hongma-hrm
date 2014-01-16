// --------------------------------------------------------------------------
// <copyright file="Holiday.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Holiday Class.
    /// </summary>
    public class Holiday : EntityBase<int>
    {
        public string HolidayName { get; set; }

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }
    }
}
