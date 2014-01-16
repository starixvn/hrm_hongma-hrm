// --------------------------------------------------------------------------
// <copyright file="ProcessCheckInOutViewModel.cs" company="Nexcel Solutions Vietnam">
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
    /// ProcessCheckInOut ViewModel.
    /// </summary>
    public class ProcessCheckInOutViewModel : BaseProcessModel
    {
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }

        public int Shift { get; set; }

        public int Type { get; set; }
    }
}
