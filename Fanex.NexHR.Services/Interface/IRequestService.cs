// --------------------------------------------------------------------------
// <copyright file="IRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IRequestService
    {
        bool Leave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        bool ChangeShift(ModelStateDictionary modelState, ChangeShiftViewModel model);

        bool LateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);

        bool CheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool Overtime(ModelStateDictionary modelState, OvertimeRequestViewModel model);

        bool UpdateLeave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        bool UpdateChangeShift(ModelStateDictionary modelState, ChangeShiftViewModel model);

        bool UpdateLateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);

        bool UpdateCheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool UpdateOvertime(ModelStateDictionary modelState, OvertimeRequestViewModel model);

        bool DeleteWorkingTimeRequest(int id);

        bool Process(string command, long id, int processBy, out string errorMessage);

        WorkingTimeRequest GetWorkingTimeRequest(long id);

        IList<RequestEntity> GetWorkingTimeRequests(RequestViewModel request, short role, bool isDailyRequest, int? loginId);

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
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

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
