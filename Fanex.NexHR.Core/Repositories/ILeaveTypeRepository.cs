namespace Fanex.NexHR.Core.Repositories
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface ILeaveTypeRepository
    {
        IList<LeaveType> GetAll();
    }
}
