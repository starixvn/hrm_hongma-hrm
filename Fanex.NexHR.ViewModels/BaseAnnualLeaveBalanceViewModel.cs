// --------------------------------------------------------------------------
// <copyright file="BaseAnnualLeaveBalanceViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// BaseAnnualLeaveBalance ViewModel.
    /// </summary>
    public class BaseAnnualLeaveBalanceViewModel : IValidatableObject
    {
        [Required]
        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Affect from")]
        public DateTime AffectedFrom { get; set; }

        public decimal DeductDays
        {
            get
            {
                return this.TotalDays < 0 ? -this.TotalDays : 0;
            }
        }

        public decimal CreditDays
        {
            get
            {
                return this.TotalDays > 0 ? this.TotalDays : 0;
            }
        }

        /// <summary>
        /// Method Validate.
        /// </summary>
        /// <param name="validationContext">Validation Context.</param>
        /// <returns>List ValidationResult.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (this.TotalDays == 0 || this.TotalDays % (decimal)0.5 != 0)
            {
                var validation = new ValidationResult(
                    Resources.Messages.InvalidInputData,
                    new[] { "TotalDays" });
                result.Add(validation);
            }

            return result;
        }
    }
}
