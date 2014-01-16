// --------------------------------------------------------------------------
// <copyright file="HolidayController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Holiday Controller.
    /// </summary>
    public class HolidayController : ApiController
    {
        private readonly IHolidayService _holidayService;

        public HolidayController()
            : this(null)
        {
        }

        public HolidayController(IHolidayService holidayRepository = null)
        {
            _holidayService = holidayRepository ?? new HolidayService();
        }

        public IList<Holiday> GetHolidays(int year)
        {
            return _holidayService.GetByYear(year);
        }
    }
}
