// --------------------------------------------------------------------------
// <copyright file="ProcessLateInEarlyOutViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// ProcessLateInEarlyOut ViewModel.
    /// </summary>
    public class ProcessLateInEarlyOutViewModel : BaseProcessModel
    {
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }

        public int Shift { get; set; }

        public int Type { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }
    }
}
