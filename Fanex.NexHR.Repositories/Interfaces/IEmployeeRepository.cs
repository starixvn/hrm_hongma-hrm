// --------------------------------------------------------------------------
// <copyright file="IEmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeRepository
    {
        Employee Get(int? employeeId = null, string domainUsername = null);

        Employee Get(string code);

        IList<Employee> Query(string query, string groupIds, bool? enable = null, bool? isDelete = null, int? titleId = null);

        IList<Employee> GetEmployeeForRelatedTeam(int employeeId);

        IList<Employee> GetAll();

        IList<Employee> GetLeaders(int id);

        int Insert(CreateEmployeeViewModel model);

        bool Delete(int id, DateTime resignedDate);

        bool Update(Employee model);

        EmployeeShift GetEmployeeShift(int id);

        bool UpdateEmployeeShift(EmployeeShift model);

        IList<WeekWorking> GetWeekWorking(int id, DateTime date);

        bool UpdateWeekWorking(EmployeeShift model, DateTime date);

        MySpaceViewModel GetMySpace(int id);

        HrLeaderDashboardModel GetHrLeaderDashboard(DateTime date, int? employeeId = null);

        IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(string date);

        bool UpdateShift(int id, DateTime date, int? shift, int modifiedBy, DateTime modifiedDate);

        IList<Employee> GetMembersByManager(int managerId, short role, string keyword, int? groupId = null);

        bool AdjustAnnualLeaves(
            string empIds, 
            string description, 
            DateTime fromDate,
            decimal deductDays, 
            decimal creditDays, 
            DateTime createdDate, 
            int createdBy);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans);

        bool DeleteAnnualLeaveTransaction(long id, DateTime modifiedDate, int modifiedBy);

        AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id);

        bool UpdateAnnualLeaveTransaction(
            long id,
            string description, 
            DateTime fromDate,
            decimal deductDays, 
            decimal creditDays, 
            DateTime modifiedDate, 
            int modifiedBy);

        bool InsertAnnualLeaveTransaction(
            int empId, 
            string description, 
            DateTime fromDate,
            decimal deductDays, 
            decimal creditDays, 
            DateTime createdDate, 
            int createdBy);

        IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees();

        IList<UserChangedShift> GetUserChangedShift(DateTime date);

        IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null);

        bool UpdateOffDayForAllEmployees(DateTime date, int isOffDay, int modifiedBy, DateTime modifiedDate, out int errorCode, out string message);

        int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DateTime> GetDisableRequestDate(int employeeId, DateTime fromDate, DateTime toDate);

        bool ResetAnnualLeaves(string empValue, string description, DateTime fromDate, int resetDays, DateTime createdDate, int createdBy);

        int ChangePassword(int empId, string password);
    }
}
