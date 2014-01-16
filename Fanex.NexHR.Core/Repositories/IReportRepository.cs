namespace Fanex.NexHR.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.ViewModels;

    public interface IReportRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <param name="empId"></param>
        /// <param name="groupId"></param>
        /// <param name="type">0: all, 1: missing checkin, 2: missing checkout, 3: late in,
        /// 4: early out, 5: not work
        /// </param>
        /// <returns></returns>
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(
            DateTime fromTime,
            DateTime toTime,
            short type,
            int? empId = null,
            int? groupId = null);
    }
}
