namespace Fanex.NexHR.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Models;


    public interface ILeaveTypeService
    {
        IList<LeaveType> GetAll();
    }
}
