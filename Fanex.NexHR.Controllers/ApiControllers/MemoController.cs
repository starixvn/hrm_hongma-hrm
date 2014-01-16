// --------------------------------------------------------------------------
// <copyright file="MemoController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Memo Controller.
    /// </summary>
    public class MemoController : ApiController
    {
        private readonly IMemoService _memoService;

        public MemoController() : this(null)
        {
        }

        public MemoController(IMemoService memoService = null)
        {
            _memoService = memoService ?? new MemoService();
        }
    }
}
