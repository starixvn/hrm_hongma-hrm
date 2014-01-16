// --------------------------------------------------------------------------
// <copyright file="IReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.20">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IReportService
    {
        IList<TotalWorkingHours> GetWorkingHoursList(
            DateTime fromDate,
            DateTime toDate,
            int? empId,
            string groupIds);

        IList<RequestsMonitoring> GetRequestMonitoring(
            DateTime fromDate,
            DateTime toDate,
            int maxRequest,
            int role,
            int employeeID);

        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(
            DateTime fromTime,
            DateTime toTime,
            short type,
            int? empId = null,
            string groupIds = null,
            int? excludeUserDelete = 1);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(
            int year,
            int? empId,
            string groupIds);

        IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> teams);
    }
}
