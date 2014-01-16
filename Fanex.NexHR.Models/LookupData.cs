// --------------------------------------------------------------------------
// <copyright file="LookupData.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Lookup Data.
    /// </summary>
    public class LookupData
    {
        public int Id { get; set; }

        public int LookupId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
