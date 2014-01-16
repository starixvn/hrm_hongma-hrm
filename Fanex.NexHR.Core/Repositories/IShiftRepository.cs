namespace Fanex.NexHR.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IShiftRepository
    {
        IList<Shift> GetAll();

        Shift GetByDate(int employeeId, DateTime date);
    }
}
