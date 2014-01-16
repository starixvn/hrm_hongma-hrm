// --------------------------------------------------------------------------
// <copyright file="ShiftController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Web.Http;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Shift Controller.
    /// </summary>
    public class ShiftController : ApiController
    {
        private readonly IShiftService _shiftService;

        public ShiftController() : this(null)
        {
        }

        public ShiftController(IShiftService shiftService = null)
        {
            _shiftService = shiftService ?? new ShiftService();
        }

        public Shift GetShiftByDate(int employeeId, DateTime date)
        {
            return _shiftService.GetByDate(employeeId, date);
        }
    }
}
