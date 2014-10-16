// --------------------------------------------------------------------------
// <copyright file="IHolidayRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IHolidayRepository
    {
        int Insert(Holiday model);

        bool Delete(int id, int modifiedBy);

        bool Update(Holiday model);

        IList<Holiday> GetByYear(int year);

        Holiday GetById(int id);
    }
}