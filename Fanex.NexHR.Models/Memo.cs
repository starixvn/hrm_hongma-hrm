// --------------------------------------------------------------------------
// <copyright file="Memo.cs" company="Nexcel Solutions Vietnam">
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
    /// Memo Class.
    /// </summary>
    public class Memo : EntityBase<int>
    {
        [Required]
        public string Contents
        {
            get;
            set;
        }

        [Required]
        public string RefNo { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string SentTo { get; set; }

        [Required]
        public string SentFrom { get; set; }

        public string ForInfo { get; set; }

        public int TotalPages { get; set; }

        [DataType(DataType.Date)]
        public DateTime SentDate { get; set; }

        public int Category { get; set; }

        public int Type { get; set; }

        public string AttachFile { get; set; }

        public string CategoryName { get; set; }

        public int CheckForEdit { get; set; }

        public bool SendMail { get; set; }
    }
}
