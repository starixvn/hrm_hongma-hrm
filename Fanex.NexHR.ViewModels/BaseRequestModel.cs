// --------------------------------------------------------------------------
// <copyright file="BaseRequestModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// BaseRequest Model.
    /// </summary>
    public class BaseRequestModel
    {
        public long Id { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public byte Status { get; set; }

        public int By { get; set; }

        public string Remark { get; set; }

        public string DisplayName { get; set; }
    }
}
