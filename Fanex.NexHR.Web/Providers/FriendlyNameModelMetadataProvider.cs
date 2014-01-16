// --------------------------------------------------------------------------
// <copyright file="FriendlyNameModelMetadataProvider.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Web.Providers
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    /// <summary>
    /// FriendlyNameModelMetadata Provider.
    /// </summary>
    public class FriendlyNameModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        // Uppercase followed by lowercase but not on existing word boundary (eg. the start)
        // Regex _camelCaseRegex = new Regex(@"\B\p{Lu}\p{Ll}", RegexOptions.Compiled);

        // Creates a nice DisplayName from the model’s property name if one hasn't been specified
        protected override ModelMetadata GetMetadataForProperty(
            Func<object> modelAccessor,
            Type containerType,
            PropertyDescriptor propertyDescriptor)
        {
            ModelMetadata metadata = base.GetMetadataForProperty(modelAccessor, containerType, propertyDescriptor);

            if (metadata.DisplayName == null)
            {
                metadata.DisplayName = DisplayNameFromCamelCase(metadata.GetDisplayName());
            }

            return metadata;
        }

        /// <summary>
        /// Display Name From CamelCase.
        /// </summary>
        /// <param name="name">Input name.</param>
        /// <returns>String value.</returns>
        private string DisplayNameFromCamelCase(string name)
        {
            var temp = "(?<!^)" + // don't match on the first character - never want to place a space here
            "(" +
            "  [A-Z][a-z] |" + // put a space before "Aaaa"
            "  (?<=[a-z])[A-Z] |" + // put a space into "aAAA" before the first capital
            "  (?<![A-Z])[A-Z]$" + // if the last letter is capital, prefix it with a space too
            ")";

            var result = Regex.Replace( 
            name,
            temp,
            " $1",
            RegexOptions.IgnorePatternWhitespace).Trim();

            return result.EndsWith(" Id", StringComparison.OrdinalIgnoreCase) ? result.Replace(" Id", string.Empty) : result;
        }
    }
}
