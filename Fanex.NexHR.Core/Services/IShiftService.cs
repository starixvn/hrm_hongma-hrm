namespace Fanex.NexHR.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IShiftService
    {
        IList<Shift> GetAll();

        Shift GetByDate(int employeeId, DateTime date);
    }
}
