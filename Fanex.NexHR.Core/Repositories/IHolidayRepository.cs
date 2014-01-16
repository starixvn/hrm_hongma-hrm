namespace Fanex.NexHR.Core.Repositories
{
    using System;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IHolidayRepository
    {
        int Insert(Holiday model);

        bool Delete(int id);

        bool Update(Holiday model);

        IList<Holiday> GetByYear(int year);

        Holiday GetById(int id);
    }
}
