// --------------------------------------------------------------------------
// <copyright file="ProcessChangeShiftViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// ProcessChangeShift ViewModel.
    /// </summary>
    public class ProcessChangeShiftViewModel : BaseProcessModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime ForDate { get; set; }

        [Required]
        public int FromShift { get; set; }

        public string FromShiftName { get; set; }

        [Required]
        public int ToShift { get; set; }

        public string ToShiftName { get; set; }

        public bool IsPlanned { get; set; }

        public string Remark { get; set; }
    }
}
