// --------------------------------------------------------------------------
// <copyright file="ChangeShiftViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// ChangeShift ViewModel.
    /// </summary>
    public class ChangeShiftViewModel : BaseRequestModel, IValidatableObject
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

        /// <summary>
        /// Method Validate.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <returns>List Validation result.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (FromShift == 0)
            {
                var validation = new ValidationResult(
                    "Invalid FromShift value",
                    new[] { "FromShift" });
                result.Add(validation);
            }

            if (ToShift == 0)
            {
                var validation = new ValidationResult(
                    "Invalid ToShift value",
                    new[] { "ToShift" });
                result.Add(validation);
            }

            if (ToShift == FromShift)
            {
                var validation = new ValidationResult(
                    Resources.Messages.ToShiftMustDifferentFromFromShift,
                    new[] { "ToShift" });
                result.Add(validation);
            }

            return result;
        }
    }
}
