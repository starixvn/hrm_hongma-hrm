// --------------------------------------------------------------------------
// <copyright file="JobTitleController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// JobTitle Controller.
    /// </summary>
    [Authorize]
    public class JobTitleController : BaseController
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

        public ActionResult JobTitleManagement()
        {
            ViewBag.JobTitles = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            return View();
        }
    }
}
