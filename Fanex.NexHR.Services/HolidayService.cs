// --------------------------------------------------------------------------
// <copyright file="HolidayService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Holiday Service.
    /// </summary>
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _holidayRepository;

        public HolidayService(IHolidayRepository holidayRepository = null)
        {
            this._holidayRepository = holidayRepository ?? new HolidayRepository();
        }

        public int Insert(Holiday holiday)
        {
            int result = 0;
            if (holiday != null)
            {
                result = this._holidayRepository.Insert(holiday);
            }

            return result;
        }

        public bool Delete(int holidayId, int modifiedBy)
        {
            bool result = false;
            if (holidayId > 0)
            {
                result = this._holidayRepository.Delete(holidayId, modifiedBy);
            }

            return result;
        }

        public bool Update(Holiday holiday)
        {
            bool result = false;
            if (holiday != null)
            {
                result = this._holidayRepository.Update(holiday);
            }

            return result;
        }

        public IList<Holiday> GetByYear(int year)
        {
            return this._holidayRepository.GetByYear(year);
        }

        public Holiday GetById(int holidayId)
        {
            if (holidayId > 0)
            {
                return this._holidayRepository.GetById(holidayId);
            }

            return new Holiday();
        }
    }
}