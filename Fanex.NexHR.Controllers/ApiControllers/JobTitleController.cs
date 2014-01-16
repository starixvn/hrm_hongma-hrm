// --------------------------------------------------------------------------
// <copyright file="JobTitleController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System.Linq;
    using System.Web.Http;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// JobTitle Controller.
    /// </summary>
    public class JobTitleController : ApiController
    {
        private readonly ILookupDataService _lookupDataService;

        public JobTitleController()
            : this(null)
        {
        }

        public JobTitleController(ILookupDataService lookupDataService)
        {
            _lookupDataService = lookupDataService ?? new LookupDataService();
        }

        public IQueryable<LookupData> GetJobTitle(string query = "")
        {
            return _lookupDataService.GetJobTitle(query).AsQueryable();
        }
    }
}
