// --------------------------------------------------------------------------
// <copyright file="IHolidayService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    public interface IHolidayService
    {
        bool Delete(int holidayId, int modifiedBy);

        Fanex.NexHR.Models.Holiday GetById(int holidayId);

        System.Collections.Generic.IList<Fanex.NexHR.Models.Holiday> GetByYear(int year);

        int Insert(Fanex.NexHR.Models.Holiday holiday);

        bool Update(Fanex.NexHR.Models.Holiday holiday);
    }
}