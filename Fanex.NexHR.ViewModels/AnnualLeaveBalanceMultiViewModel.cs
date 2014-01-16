// --------------------------------------------------------------------------
// <copyright file="AnnualLeaveBalanceMultiViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// AnnualLeaveBalanceMulti ViewModel.
    /// </summary>
    public class AnnualLeaveBalanceMultiViewModel : BaseAnnualLeaveBalanceViewModel
    {
        [Required]
        public string EmpIds { get; set; }
    }
}
