// --------------------------------------------------------------------------
// <copyright file="MemoViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Memo ViewModel.
    /// </summary>
    public class MemoViewModel
    {
        public string Content { get; set; }

        public int Year { get; set; }

        public int Category { get; set; }
    }
}
