// --------------------------------------------------------------------------
// <copyright file="OvertimeRequestViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.3">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// OvertimeRequest ViewModel.
    /// </summary>
    public class OvertimeRequestViewModel : BaseRequestModel, IValidatableObject
    {
        [Required]
        public short OvertimeType { get; set; }

        [Required]
        public bool IsOvertimeHours { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public int Shift { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public bool IsCrossDay { get; set; }

        public int TotalMinutes { get; set; }

        public bool IsPlanned { get; set; }

        /// <summary>
        /// Method Validate.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <returns>List Validation result.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();
            if (this.IsOvertimeHours)
            {
                if (FromTime == ToTime)
                {
                    var validation = new ValidationResult(
                        "Invalid Time value",
                        new[] { "Time" });
                    result.Add(validation);
                }

                if (TotalMinutes < 0)
                {
                    var validation = new ValidationResult(
                        "Invalid Time value",
                        new[] { "Time" });
                    result.Add(validation);
                }
            }
            else
            {
                if (Shift == 0)
                {
                    var validation = new ValidationResult(
                        "Invalid Shift value",
                        new[] { "Shift" });
                    result.Add(validation);
                }
            }

            return result;
        }
    }
}
