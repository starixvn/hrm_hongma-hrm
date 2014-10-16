// --------------------------------------------------------------------------
// <copyright file="IReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(
            DateTime fromTime,
            DateTime toTime,
            short type,
            int? empId = null,
            string groupIds = null,
            int? excludeUserDelete = 1);

        IList<RequestsMonitoring> GetRequestMonitoring(
            DateTime fromDate,
            DateTime toDate,
            int maxRequest,
            int role, 
            int employeeID);

        IList<TotalWorkingHours> GetWorkingHoursList(
            DateTime fromDate,
            DateTime toDate,
            int? empId,
            string groupIds);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(
            int year,
            int? empId,
            string groupIds);

        IList<ExtensionNoModel> GetAllExtensionNo();

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(bool isInternal, int? groupId);

        bool AddNewExtensionNoMember(ExtensionNoModel model, out int errorCode, out string message);

        ExtensionNoModel GetEmployeeExtensionNoById(bool isInternal, int empExtID);

        bool UpdateExtensionNoMember(ExtensionNoModel model, out int errorCode, out string message);

        bool DeleteExtensionNumber(int extId, out int errorCode, out string message);

        bool AddNewExternalGroup(string groupName, out int errorCode, out string message);

        bool EditExternalGroup(int groupId, string groupName, out int errorCode, out string message);

        bool DeleteExternalGroup(int groupId, out int errorCode, out string message);
    }
}
