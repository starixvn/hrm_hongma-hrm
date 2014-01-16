// --------------------------------------------------------------------------
// <copyright file="IRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.9">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface IRequestRepository
    {
        long WorkingTimeRequest(
            int empId,
            short requestType,
            bool isPlanned,
            string remark,
            DateTime createdDate,
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
            int? earlyOutMins = null,
            short? overtimeType = null,
            bool isOvertimeHours = false);

        bool UpdateRequest(
            long requestId,
            bool isPlanned,
            string remark,
            int modifiedby,
            DateTime fromDate,
            DateTime modifiedDate,
            int? fromShift = null,
            DateTime? toDate = null,
            int? toShift = null,
            int? leaveTypeId = null,
            decimal? totalDays = null,
            int? lateInMins = null,
            int? earlyOutMins = null,
            short? overtimeType = null,
            bool isOvertimeHours = false);

        bool DeleteWorkingTimeRequest(int id);

        int ProcessRequest(
            Commands command,
            long requestId,
            int processedBy,
            DateTime processedDate,
            out string message);

        IList<WorkingTimeRequest> GetWorkingTimeRequests(int id, int? requestType = null, byte? status = null);

        WorkingTimeRequest GetWorkingTimeRequest(long workingTimeId);

        IList<RequestEntity> GetAllRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetLeaveRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetChangeShiftRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetLateInEarlyOutRequest(
            short roleID,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? longInId = null);

        IList<RequestEntity> GetCheckInOutRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetOvertimeRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetLeaveRequestHistory(long requestId, int requestTypeId);

        IList<RequestEntity> GetChangeShiftRequestHistory(long requestId, int requestTypeId);

        IList<RequestEntity> GetLateInEarlyOutRequestHistory(long requestId, int requestTypeId);

        IList<RequestEntity> GetCheckInOutRequestHistory(long requestId, int requestTypeId);

        IList<RequestEntity> GetOvertimeRequestHistory(long requestId, int requestTypeId);
    }
}
