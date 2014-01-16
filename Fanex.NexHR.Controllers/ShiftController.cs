// --------------------------------------------------------------------------
// <copyright file="ShiftController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Shift Controller.
    /// </summary>
    [Authorize]
    public class ShiftController : Controller
    {
        private readonly IShiftService _shiftService;

        public ShiftController()
            : this(null)
        {
        }

        public ShiftController(IShiftService shiftService = null)
        {
            _shiftService = shiftService ?? new ShiftService();
        }

        public ActionResult Index()
        {
            return View(_shiftService.GetAll());
        }
    }
}
