// --------------------------------------------------------------------------
// <copyright file="SearchEmployeeModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// SearchEmployee Model.
    /// </summary>
    public class SearchEmployeeModel
    {
        public string Query { get; set; }

        public int? Team { get; set; }
    }
}
