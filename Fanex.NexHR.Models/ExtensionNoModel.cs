// --------------------------------------------------------------------------
// <copyright file="ExtensionNoModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.10">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Extension Number Model.
    /// </summary>
    public class ExtensionNoModel
    {
        public int EmpExtID { get; set; }

        [Required]
        public int TeamId { get; set; }

        public string Team { get; set; }

        public string TeamName { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        [Required]
        public string ExtensionNo { get; set; }

        public int OrderNoGroup { get; set; }

        public int OrderNoMember { get; set; }

        public bool IsInternal { get; set; }

        [Required(ErrorMessage = "The Employee field is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The Employee field is required.")]
        public int EmpId { get; set; }

        public string Employee { get; set; }

        public int TotalMember { get; set; }
    }
}
