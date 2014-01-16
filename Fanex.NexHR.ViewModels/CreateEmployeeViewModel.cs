// --------------------------------------------------------------------------
// <copyright file="CreateEmployeeViewModel.cs" company="Nexcel Solutions Vietnam">
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
    using System.Web.Mvc;

    /// <summary>
    /// CreateEmployee ViewModel.
    /// </summary>
    public class CreateEmployeeViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string NickName { get; set; }
        [Required]
        public string DomainUsername { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public int? MonShift { get; set; }

        public int? TueShift { get; set; }

        public int? WedShift { get; set; }

        public int? ThuShift { get; set; }

        public int? FriShift { get; set; }

        public int? SatShift { get; set; }

        public int? SunShift { get; set; }
        [Required]
        public int TeamId { get; set; }

        public int ProcessBy { get; set; }
    }
}