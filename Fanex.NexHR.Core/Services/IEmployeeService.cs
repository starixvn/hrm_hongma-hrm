namespace Fanex.NexHR.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeService
    {
        Employee Get(int employeeId);

        Employee Get(string domainUsername);

        IList<Employee> Query(string query, int? groupId);

        IList<Employee> GetAll();

        IList<Employee> GetLeaders(int id);

        int Insert(CreateEmployeeViewModel model);

        bool Delete(int id);

        bool Update(Employee model);

        EmployeeShift GetEmployeeShift(int id);

        bool UpdateEmployeeShift(EmployeeShift model);

        IList<WeekWorking> GetWeekWorking(int id, DateTime date);

        bool UpdateWeekWorking(EmployeeShift model, DateTime date);

        long WorkingTimeRequest(
            int empId,
            short requestType,
            bool isPlanned,
            string remark,
            int createdBy,
            out int errorCode,
            out string message,
            DateTime fromDate,
            int? fromShift = null,
            DateTime? toDate = null,
            int? toShift = null,
            int? leaveTypeId = null,
            decimal? totalDays = null,
            int? lateInMins = null,
            int? earlyOutMins = null);

        int ApproveRequest(
            long requestId,
            int approvedBy,
            DateTime approvedDate,
            int modifiedBy,
            out string message);

        int DisapproveRequest(long requestId, int modifiedBy, DateTime approvedDate, out string message);

        int RejectRequest(
            long requestId,
            int processedBy,
            DateTime processedDate,
            int modifiedBy,
            out string message);

        bool UpdateRequest(
            long requestId,
            bool isPlanned,
            string remark,
            int modifiedby,
            DateTime fromDate,
            int? fromShift = null,
            DateTime? toDate = null,
            int? toShift = null,
            int? leaveTypeId = null,
            decimal? totalDays = null,
            int? lateInMins = null,
            int? earlyOutMins = null);

        /// <summary>
        /// Add by Shadow
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MySpaceViewModel GetMySpace(int id);

        /// <summary>
        /// Add by shadow
        /// </summary>
        /// <param name="date"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id);

        /// <summary>
        /// Add by Shadow
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        IList<WorkingCalendarsViewModel> GetWorkingCalendars(string date);

        IList<WorkingTimeRequest> GetWorkingTimeRequests(int id, int? requestType = null, byte? status = null);

        WorkingTimeRequest GetWorkingTimeRequest(long id);

        bool DeleteWorkingTimeRequest(int id);

        bool UpdateShift(int id, DateTime date, int? shift);

        IList<CheckInOut> GetCheckInOutRequest(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null);
    }
}
