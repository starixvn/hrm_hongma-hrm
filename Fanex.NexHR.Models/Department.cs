// --------------------------------------------------------------------------
// <copyright file="Department.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Department Class.
    /// </summary>
    public class Department : EntityBase<int>
    {
        public Employee Manager { get; set; }
    }
}
