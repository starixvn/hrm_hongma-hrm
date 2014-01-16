// --------------------------------------------------------------------------
// <copyright file="RequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// RequestService Class.
    /// </summary>
    public class RequestService : IRequestService
    {
        #region constant
        private const string _MsgLeave = "<span class='req leave'>{1}</span> {0} days";
        private const string _MsgLeaveHalfDay = "<span class='req leave'>{0} ({1}-{2})</span> 0.5 day";
        private const string _MsgLeaveOneDay = "<span class='req leave'>{0}</span> 1 day";
        private const string _MsgLeaveDuringWorking = "<span class='req leave'>{0} ({1}-{2})</span> {3} munites";
        private const string _MsgChangeShift = "<span class='req changeshift'>{0} <span class='arr'>&rarr;</span> {1}</span> {2}";
        private const string _MsgCheckInOut = "<span class='req checkinout'>{0} {1}</span> {2}";
        private const string _MsgLateIn = "<span class='req inout'>+{0}' {1}</span> {2}";
        private const string _MsgEarlyOut = "<span class='req inout'>-{0}' {1}</span> {2}";
        private const string _MsgOvertimeHours = "<span class='req overtime'>{0} ({1}-{2})</span> {3}' - {4}";
        private const string _MsgOvertimeShift = "<span class='req overtime'>{0} ({1})</span> {2}";

        private const string _MailContentLeave = "{0} to {1} ({2} days)";
        private const string _MailContentLeaveOneDay = "{0}";
        private const string _MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";
        private const string _MailContentLeaveDuringWorking = "{0} ({1} - {2})";
        private const int _MaxMunites = 210;

        #endregion

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IShiftService _shiftService;
        private readonly IRequestRepository _requestRepository;
        private string _baseUrl;

        public RequestService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._requestRepository = new RequestRepository();
            this._emailService = new EmailService();
            this._leaveTypeService = new LeaveTypeService();
            this._shiftService = new ShiftService();
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public RequestService(
            IEmployeeRepository employeeRepository,
            IEmailService emailService,
            ILeaveTypeService leaveTypeService,
            IRequestRepository requestRepository,
            IShiftService shiftService)
        {
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
            this._leaveTypeService = leaveTypeService;
            this._requestRepository = requestRepository;
            this._shiftService = shiftService;
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// Get info detail for request.
        /// </summary>
        /// <param name="request">RequestEntity Object.</param>
        /// <returns>Detail request.</returns>
        public static string ToRequestMessage(RequestEntity request)
        {
            StringBuilder msg = new StringBuilder();
            var message = string.Empty;
            switch (request.Type.Id)
            {
                case (int)RequestTypes.Leave:
                    msg.Append(GetMessageForLeave(request));
                    break;
                case (int)RequestTypes.ChangeShift:
                    msg.AppendFormat(_MsgChangeShift, request.FromShift.Name, request.ToShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
                case (int)RequestTypes.CheckIn:
                case (int)RequestTypes.CheckOut:
                    msg.AppendFormat(_MsgCheckInOut, HourFormat(request.FromDate), request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
                case (int)RequestTypes.LateIn:
                    msg.AppendFormat(_MsgLateIn, request.TotalMins, request.FromShift.Name, GetMessageDateforLeaveRequest(request.FromDate, request.ToDate));
                    break;
                case (int)RequestTypes.EarlyOut:
                    msg.AppendFormat(_MsgEarlyOut, request.TotalMins, request.FromShift.Name, GetMessageDateforLeaveRequest(request.FromDate, request.ToDate));
                    break;
                case (int)RequestTypes.Overtime:
                    msg.Append(GetMessageForOvertime(request));
                    break;
                default:
                    break;
            }

            return msg.ToString();
        }

        /// <summary>
        /// Get data date detail for leave request.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">The To date.</param>
        /// <returns>Date detail.</returns>
        public static string GetMessageDateforLeaveRequest(DateTime fromDate, DateTime toDate)
        {
            string result = string.Empty;
            if ((toDate - fromDate).Days > 0)
            {
                if (fromDate.Year == toDate.Year)
                {
                    if (fromDate.Month != toDate.Month)
                    {
                        result = fromDate.ToString("dd MMM") + " - " + toDate.ToDateMonthYearFormatedDate();
                    }
                    else
                    {
                        result = fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();
                    }
                }
                else
                {
                    result = fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
                }
            }
            else
            {
                result = fromDate.ToDateMonthYearFormatedDate();
            }

            return result;
        }

        /// <summary>
        /// Insert Leave request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">LeaveRequest ViewModel.</param>
        /// <returns>Result of insert.</returns>
        public bool Leave(ModelStateDictionary modelState, LeaveRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                if (!CheckIsValidLeaveRequest(modelState, model))
                {
                    return false;
                }

                if (model.IsLeaveDuringWorking)
                {
                    model.TotalDays = 0;
                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                }

                var requestId = this._requestRepository.WorkingTimeRequest(
                    model.EmployeeId,
                    (short)RequestTypes.Leave,
                    model.IsPlanned,
                    model.Remark,
                    DateTime.Now,
                    model.By,
                    out errorCode,
                    out message,
                    fromDate: model.FromDate,
                    toDate: model.ToDate,
                    totalDays: model.TotalDays,
                    leaveTypeId: model.LeaveTypeId);
                if (errorCode == 0)
                {
                    SendMailForLeaveSuccess(model, requestId, isUpdate: false);
                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Insert Change Shift request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">ChangeShift ViewModel.</param>
        /// <returns>Result of insert.</returns>
        public bool ChangeShift(ModelStateDictionary modelState, ChangeShiftViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                var requestId = this._requestRepository.WorkingTimeRequest(
                    model.EmployeeId,
                    (short)RequestTypes.ChangeShift,
                    model.IsPlanned,
                    model.Remark,
                    DateTime.Now,
                    model.By,
                    out errorCode,
                    out message,
                    model.ForDate,
                    fromShift: model.FromShift,
                    toShift: model.ToShift);
                if (errorCode == 0)
                {
                    //// get leader(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    if (leaders == null || leaders.Count == 0)
                    {
                        string errorMessage;
                        this.Process(Commands.Approve.EnumToString(), requestId, model.EmployeeId, out errorMessage);
                    }
                    else
                    {
                        var templateEmailPath = HttpContext.Current.Server.MapPath("~/app_data/emails/ChangeShift.Request.cshtml");
                        var taskSendMail = new Task(() => this.SendMailForChangeShiftRequest(model, requestId, false, templateEmailPath, leaders));
                        taskSendMail.Start();
                    }

                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Insert Late In, Early Out request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <returns>Result of insert.</returns>
        public bool LateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model)
        {
            if (!CheckIsValidLateInEarlyOutRequest(modelState, model))
            {
                return false;
            }
            
            int errorCode;
            string message;
            int? lateInMins = model.Type == (short)RequestTypes.LateIn ? (int?)model.Minute : null;
            int? earlyOutMins = model.Type == (short)RequestTypes.EarlyOut ? (int?)model.Minute : null;
            if (!string.IsNullOrEmpty(model.FromTime) && !string.IsNullOrEmpty(model.ToTime))
            {
                model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
            }

            var requestId = this._requestRepository.WorkingTimeRequest(
                model.EmployeeId,
                model.Type,
                false,
                model.Remark,
                DateTime.Now,
                model.By,
                out errorCode,
                out message,
                model.FromDate,
                toDate: model.ToDate,
                lateInMins: lateInMins,
                earlyOutMins: earlyOutMins);
            if (errorCode == 0)
            {
                SendMailForLateInEarlyOutSuccess(model, requestId, isUpdate: false);
                return true;
            }
            else
            {
                modelState.AddModelError(string.Empty, message);
            }
                    
            return false;
        }

        /// <summary>
        /// Insert Check In/Out request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">CheckInOutRequest ViewModel.</param>
        /// <returns>Result of insert.</returns>
        public bool CheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                // update date & time
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
                int errorCode;
                string message;
                var requestId = this._requestRepository.WorkingTimeRequest(
                    model.EmployeeId,
                    model.Type,
                    false,
                    null,
                    DateTime.Now,
                    model.By,
                    out errorCode,
                    out message,
                    model.ForDate);
                if (errorCode == 0)
                {
                    //// get leader(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    if (leaders == null || leaders.Count == 0)
                    {
                        string errorMessage;
                        this.Process(Commands.Approve.EnumToString(), requestId, model.EmployeeId, out errorMessage);
                    }
                    else
                    {
                        var templateEmailPath = HttpContext.Current.Server.MapPath("~/app_data/emails/CheckInOut.Request.cshtml");
                        var taskSendMail = new Task(() => this.SendMailForCheckInOutRequest(model, requestId, false, templateEmailPath, leaders));
                        taskSendMail.Start();
                    }

                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Insert Overtime request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">OvertimeRequest ViewModel.</param>
        /// <returns>Result of insert.</returns>
        public bool Overtime(ModelStateDictionary modelState, OvertimeRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                var fromDate = model.Date;
                var toDate = model.Date;
                if (model.IsOvertimeHours)
                {
                    fromDate = Utilities.AddTimeToDate(model.Date, model.FromTime);
                    toDate = model.IsCrossDay ? Utilities.AddTimeToDate(model.Date.AddDays(1), model.ToTime) : Utilities.AddTimeToDate(model.Date, model.ToTime);
                }

                var requestId = this._requestRepository.WorkingTimeRequest(
                    model.EmployeeId,
                    (short)RequestTypes.Overtime,
                    model.IsPlanned,
                    model.Remark,
                    DateTime.Now,
                    model.By,
                    out errorCode,
                    out message,
                    fromDate: fromDate,
                    toDate: toDate,
                    fromShift: model.Shift,
                    totalDays: model.TotalMinutes,
                    overtimeType: model.OvertimeType,
                    isOvertimeHours: model.IsOvertimeHours);
                if (errorCode == 0)
                {
                    SendMailForOvertimeSuccess(model, requestId, isUpdate: false);
                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Update Leave request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">LeaveRequest ViewModel.</param>
        /// <returns>Result of update.</returns>
        public bool UpdateLeave(ModelStateDictionary modelState, LeaveRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                if (model.HalfDayOff)
                {
                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                    if (model.TotalDays != (decimal)0.5)
                    {
                        modelState.AddModelError("TotalDays", Messages.InvalidTotalDays);
                        return false;
                    }
                }
                else if (model.TotalDays % 1 != 0)
                {
                    modelState.AddModelError("TotalDays", Messages.InvalidTotalDays);
                    return false;
                }

                if (model.IsLeaveDuringWorking)
                {
                    model.TotalDays = 0;
                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                }

                var updated = this._requestRepository.UpdateRequest(
                    model.Id,
                    model.IsPlanned,
                    model.Remark,
                    model.EmployeeId,
                    model.FromDate,
                    DateTime.Now,
                    toDate: model.ToDate,
                    leaveTypeId: model.LeaveTypeId,
                    totalDays: model.TotalDays);
                if (updated)
                {
                    SendMailForLeaveSuccess(model, model.Id, isUpdate: true);
                }

                return updated;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Update Change Shift request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">ChangeShift ViewModel.</param>
        /// <returns>Result of update.</returns>
        public bool UpdateChangeShift(ModelStateDictionary modelState, ChangeShiftViewModel model)
        {
            if (modelState.IsValid)
            {
                var updated = this._requestRepository.UpdateRequest(
                    model.Id,
                    model.IsPlanned,
                    model.Remark,
                    model.By,
                    model.ForDate,
                    DateTime.Now,
                    fromShift: model.FromShift,
                    toShift: model.ToShift);

                if (updated)
                {
                    // get leader(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    if (leaders == null || leaders.Count == 0)
                    {
                        string errorMessage;
                        this.Process(Commands.Approve.EnumToString(), model.Id, model.EmployeeId, out errorMessage);
                    }
                    else
                    {
                        var templateEmailPath = HttpContext.Current.Server.MapPath("~/app_data/emails/ChangeShift.Request.cshtml");
                        var taskSendMail = new Task(() => this.SendMailForChangeShiftRequest(model, model.Id, true, templateEmailPath, leaders));
                        taskSendMail.Start();
                    }
                }

                return updated;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Update Late In, Early Out request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <returns>Result of update.</returns>
        public bool UpdateLateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                if (model.ToDate >= model.FromDate)
                {
                    if (model.Minute > 0 && model.Minute <= 210)
                    {
                        int? lateInMins = model.Type == (short)RequestTypes.LateIn ? (int?)model.Minute : null;
                        int? earlyOutMins = model.Type == (short)RequestTypes.EarlyOut ? (int?)model.Minute : null;
                        if (!string.IsNullOrEmpty(model.FromTime) && !string.IsNullOrEmpty(model.ToTime))
                        {
                            model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                            model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                        }

                        var updated = this._requestRepository.UpdateRequest(
                            model.Id,
                            false,
                            model.Remark,
                            model.By,
                            model.FromDate,
                            toDate: model.ToDate,
                            modifiedDate: DateTime.Now,
                            lateInMins: lateInMins,
                            earlyOutMins: earlyOutMins);
                        if (updated)
                        {
                            SendMailForLateInEarlyOutSuccess(model, model.Id, isUpdate: true);
                        }

                        return updated;
                    }
                    else
                    {
                        modelState.AddModelError(string.Empty, Resources.Messages.WrongMunites);
                    }
                }
                else
                {
                    modelState.AddModelError(string.Empty, Resources.Messages.WrongDateRange);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Update Check In/Out request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">CheckInOutRequest ViewModel.</param>
        /// <returns>Result of update.</returns>
        public bool UpdateCheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);

                var updated = this._requestRepository.UpdateRequest(
                    model.Id,
                    false,
                    null,
                    model.EmployeeId,
                    model.ForDate,
                    DateTime.Now);

                if (updated)
                {
                    // get leader(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    if (leaders == null || leaders.Count == 0)
                    {
                        string errorMessage;
                        this.Process(Commands.Approve.EnumToString(), model.Id, model.EmployeeId, out errorMessage);
                    }
                    else
                    {
                        var templateEmailPath = HttpContext.Current.Server.MapPath("~/app_data/emails/CheckInOut.Request.cshtml");
                        var taskSendMail = new Task(() => this.SendMailForCheckInOutRequest(model, model.Id, true, templateEmailPath, leaders));
                        taskSendMail.Start();
                    }
                }

                return updated;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Update Overtime request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">OvertimeRequest ViewModel.</param>
        /// <returns>Result of update.</returns>
        public bool UpdateOvertime(ModelStateDictionary modelState, OvertimeRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                var fromDate = model.Date;
                var toDate = model.Date;
                if (model.IsOvertimeHours)
                {
                    fromDate = Utilities.AddTimeToDate(model.Date, model.FromTime);
                    toDate = model.IsCrossDay ? Utilities.AddTimeToDate(model.Date.AddDays(1), model.ToTime) : Utilities.AddTimeToDate(model.Date, model.ToTime);
                }

                var updated = this._requestRepository.UpdateRequest(
                    model.Id,
                    model.IsPlanned,
                    model.Remark,
                    model.EmployeeId,
                    fromDate,
                    DateTime.Now,
                    toDate: toDate,
                    fromShift: model.Shift,
                    totalDays: model.TotalMinutes,
                    overtimeType: model.OvertimeType,
                    isOvertimeHours: model.IsOvertimeHours);
                if (updated)
                {
                    SendMailForOvertimeSuccess(model, model.Id, isUpdate: true);
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);
                    if (leaders == null || leaders.Count == 0)
                    {
                        string errorMessage;
                        this.Process(Commands.Approve.EnumToString(), model.Id, model.EmployeeId, out errorMessage);
                    }
                    else
                    {
                        var templateEmailPath = HttpContext.Current.Server.MapPath("~/app_data/emails/Overtime.Request.cshtml");
                        var taskSendMail = new Task(() => this.SendMailForOvertimeRequest(model, model.Id, true, templateEmailPath, leaders));
                        taskSendMail.Start();
                    }
                }

                return updated;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Approve, disapprove or reject a request,
        /// notification email will be sent also.
        /// </summary>
        /// <param name="command">Action on this request.</param>
        /// <param name="id">Request id.</param>
        /// <param name="processBy">Process By.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Return boolean.</returns>
        public bool Process(string command, long id, int processBy, out string errorMessage)
        {
            var request = this._requestRepository.GetWorkingTimeRequest(id);
            var employee = this._employeeRepository.Get(request.EmpId);
            var processUser = this._employeeRepository.Get(processBy);

            Commands processCommand;
            switch (command)
            {
                case "Approve":
                    processCommand = Commands.Approve;
                    break;
                case "Disapprove":
                    processCommand = Commands.Disapprove;
                    break;
                case "Reject":
                    processCommand = Commands.Reject;
                    break;
                default:
                    processCommand = Commands.Approve;
                    break;
            }

            string message;
            int code = this._requestRepository.ProcessRequest(
                processCommand, id, processBy, DateTime.Now, out message);

            if (code == 0)
            {
                var templateEmailPathForRequester = HttpContext.Current.Server.MapPath("~/app_data/emails/process.request.cshtml");
                var templateEmailPathForRelated = GetEmailTemplatePath(request.RequestType);

                var taskSendMail = new Task(()
                    => this.SendMailProcess(command, request, employee, processUser, processCommand, templateEmailPathForRequester, templateEmailPathForRelated));
                taskSendMail.Start();
            }
            else
            {
                errorMessage = message;
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Function to get working time request by id.
        /// </summary>
        /// <param name="workingTimeId">WorkingTime Request Id.</param>
        /// <returns>WorkingTimeRequest Object.</returns>
        public WorkingTimeRequest GetWorkingTimeRequest(long workingTimeId)
        {
            if (workingTimeId > 0)
            {
                return this._requestRepository.GetWorkingTimeRequest(workingTimeId);
            }

            return null;
        }

        /// <summary>
        /// Function to delete a working time request.
        /// </summary>
        /// <param name="id">WorkingTime Request Id.</param>
        /// <returns>Result of deleted.</returns>
        public bool DeleteWorkingTimeRequest(int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");

            return this._requestRepository.DeleteWorkingTimeRequest(id);
        }

        /// <summary>
        /// Function to get check in, check out request.
        /// </summary>
        /// <param name="roleId">Role of user.</param>
        /// <param name="fromDate">The From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">List group Id.</param>
        /// <param name="isDailyRequest">Is query from daily request.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="status">Status of request.</param>
        /// <param name="loginId">User login.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetCheckInOutRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? loginId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                if (fromDate <= toDate)
                {
                    entityList = this._requestRepository.GetCheckInOutRequest(roleId, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get late in, early out request.
        /// </summary>
        /// <param name="roleId">Role of user.</param>
        /// <param name="fromDate">The From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">List group Id.</param>
        /// <param name="isDailyRequest">Is query from daily request.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="status">Status of request.</param>
        /// <param name="loginId">User login.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetLateInEarlyOutRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? loginId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                if (fromDate <= toDate)
                {
                    entityList = this._requestRepository.GetLateInEarlyOutRequest(roleId, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get change shift request.
        /// </summary>
        /// <param name="roleId">Role of user.</param>
        /// <param name="fromDate">The From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">List group Id.</param>
        /// <param name="isDailyRequest">Is query from daily request.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="status">Status of request.</param>
        /// <param name="loginId">User login.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetChangeShiftRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? loginId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                if (fromDate <= toDate)
                {
                    entityList = this._requestRepository.GetChangeShiftRequest(roleId, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get leave request.
        /// </summary>
        /// <param name="roleId">Role of user.</param>
        /// <param name="fromDate">The From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">List group Id.</param>
        /// <param name="isDailyRequest">Is query from daily request.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="status">Status of request.</param>
        /// <param name="loginId">User login.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetLeaveRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? loginId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                if (fromDate <= toDate)
                {
                    entityList = this._requestRepository.GetLeaveRequest(roleId, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get overtime request.
        /// </summary>
        /// <param name="roleId">Role of user.</param>
        /// <param name="fromDate">The From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">List group Id.</param>
        /// <param name="isDailyRequest">Is query from daily request.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="status">Status of request.</param>
        /// <param name="loginId">User login.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetOvertimeRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? loginId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                if (fromDate <= toDate)
                {
                    entityList = this._requestRepository.GetOvertimeRequest(roleId, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get all request.
        /// </summary>
        /// <param name="roleId">Role of user.</param>
        /// <param name="fromDate">The From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">List group Id.</param>
        /// <param name="isDailyRequest">Is query from daily request.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="status">Status of request.</param>
        /// <param name="loginId">User login.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetAllRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? loginId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                if (fromDate <= toDate)
                {
                    entityList = this._requestRepository.GetAllRequest(roleId, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get working time request.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <param name="role">Role of user.</param>
        /// <param name="isDailyRequest">Is query from daily request.</param>
        /// <param name="loginId">User login.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetWorkingTimeRequests(RequestViewModel request, short role, bool isDailyRequest, int? loginId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();

            var status = request.StatusId != 0 ? new byte?((byte)request.StatusId) : null;
            var employeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null;

            switch (request.RequestType)
            {
                case (int)RequestFilter.Leave:
                    entityList = GetLeaveRequest(role, request.FromDate, request.ToDate, request.TeamIds, isDailyRequest, employeeId, status, loginId);
                    break;
                case (int)RequestFilter.ChangeShift:
                    entityList = GetChangeShiftRequest(role, request.FromDate, request.ToDate, request.TeamIds, isDailyRequest, employeeId, status, loginId);
                    break;
                case (int)RequestFilter.LateInEarlyOut:
                    entityList = GetLateInEarlyOutRequest(role, request.FromDate, request.ToDate, request.TeamIds, isDailyRequest, employeeId, status, loginId);
                    break;
                case (int)RequestFilter.CheckInOut:
                    entityList = GetCheckInOutRequest(role, request.FromDate, request.ToDate, request.TeamIds, isDailyRequest, employeeId, status, loginId);
                    break;
                case (int)RequestFilter.Overtime:
                    entityList = GetOvertimeRequest(role, request.FromDate, request.ToDate, request.TeamIds, isDailyRequest, employeeId, status, loginId);
                    break;
                case (int)RequestFilter.All:
                    entityList = GetAllRequest(role, request.FromDate, request.ToDate, request.TeamIds, isDailyRequest, employeeId, status, loginId);
                    break;
                default:
                    break;
            }

            return entityList;
        }

        /// <summary>
        /// Function to get Leave request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetLeaveRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._requestRepository.GetLeaveRequestHistory(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get Change shift request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetChangeShiftRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._requestRepository.GetChangeShiftRequestHistory(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get Late in, early out request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetLateInEarlyOutRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._requestRepository.GetLateInEarlyOutRequestHistory(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get Check in, check out request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetCheckInOutRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._requestRepository.GetCheckInOutRequestHistory(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get Overtime request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetOvertimeRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._requestRepository.GetOvertimeRequestHistory(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                /*writelog*/
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get Hour format from date.
        /// </summary>
        /// <param name="date">Input a date.</param>
        /// <returns>String format of date.</returns>
        private static string HourFormat(DateTime date)
        {
            return date.ToString("HH:mm");
        }

        /// <summary>
        /// Function to get message for leave request.
        /// </summary>
        /// <param name="request">Request Entity.</param>
        /// <returns>Return detail of leave request.</returns>
        private static string GetMessageForLeave(RequestEntity request)
        {
            var messageDetail = string.Empty;
            var leaveDate = GetMessageDateforLeaveRequest(request.FromDate, request.ToDate);

            if (request.TotalDays > 1)
            {
                messageDetail = string.Format(_MsgLeave, Convert.ToInt32(request.TotalDays), leaveDate);
            }
            else if (request.TotalDays == 1)
            {
                messageDetail = string.Format(_MsgLeaveOneDay, GetMessageDateforLeaveRequest(request.FromDate, request.ToDate));
            }
            else if ((request.TotalDays - (decimal)0.5) == 0)
            {
                messageDetail = string.Format(
                    _MsgLeaveHalfDay, GetMessageDateforLeaveRequest(request.FromDate, request.ToDate), HourFormat(request.FromDate), HourFormat(request.ToDate));
            }
            else
            {
                messageDetail = string.Format(
                    _MsgLeaveDuringWorking, leaveDate, HourFormat(request.FromDate), HourFormat(request.ToDate), (request.ToDate - request.FromDate).TotalMinutes);
            }

            return messageDetail;
        }

        /// <summary>
        /// Function to get message for overtime request.
        /// </summary>
        /// <param name="request">Request Entity.</param>
        /// <returns>Return detail of overtime request.</returns>
        private static string GetMessageForOvertime(RequestEntity request)
        {
            var messageDetail = string.Empty;
            var overtime = new OvertimeType { Id = request.OvertimeType }.Name;
            if (request.IsOvertimeHours)
            {
                messageDetail = string.Format(
                    _MsgOvertimeHours, 
                    request.FromDate.ToDateMonthYearFormatedDate(), 
                    HourFormat(request.FromDate), 
                    HourFormat(request.ToDate), 
                    request.TotalDays.ToString("0.#"), 
                    overtime);
            }
            else
            {
                var shift = request.FromShift != null ? request.FromShift.Name : string.Empty;
                messageDetail = string.Format(_MsgOvertimeShift, request.FromDate.ToDateMonthYearFormatedDate(), shift, overtime);
            }

            return messageDetail;
        }

        private void SendMailProcess(
            string command,
            WorkingTimeRequest request,
            Employee employee,
            Employee processUser,
            Commands processCommand,
            string templateEmailPathForRequester,
            string templateEmailPathForRelated)
        {
            this.SendToRequester(command, request, employee, processUser, templateEmailPathForRequester);

            if (processCommand == Commands.Approve)
            {
                this.SendToRelatedPeople(processUser, request, employee, templateEmailPathForRelated);
            }
        }

        /// <summary>
        /// Function to send mail to requester.
        /// </summary>
        /// <param name="command">Type of changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which change status request.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void SendToRequester(string command, WorkingTimeRequest request, Employee employee, Employee processUser, string templateEmailPath)
        {
            var allShifts = this._shiftService.GetAll();
            string forDate = string.Empty;
            if (request.TotalDays > 1)
            {
                forDate = " from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " + request.ToDate.ToDateMonthYearFormatedDate();
            }
            else
            {
                forDate = " on " + request.FromDate.ToDateMonthYearFormatedDate();
            }

            // send email to the person who made this request
            var bodyContent = new
            {
                Employee = employee.NickName,
                RequestId = request.Id,
                Action = command == Commands.Reject.EnumToString()
                ? Resources.Labels.Rejected
                : (command == Commands.Approve.EnumToString()
                    ? Resources.Labels.Approved
                    : Resources.Labels.Disapproved),
                ForDate = forDate,
                Request = request.RequestType.ToText(),
                User = processUser.NickName,
                DisplayNameUser = processUser.FullName,
                Url = this._baseUrl
            };

            var body = Utilities.ReadTextFile(templateEmailPath);
            body = RazorEngine.Razor.Parse(body, bodyContent);

            var subject = string.Format(
                Resources.Messages.ProcessSubject,
                request.RequestType.ToText(),
                forDate,
                bodyContent.Action,
                processUser.NickName);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                employee.Email,
                string.Empty,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail to other staff which related group.
        /// </summary>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void SendToRelatedPeople(Models.Employee processUser, Models.WorkingTimeRequest request, Models.Employee employee, string templateEmailPath)
        {
            // send notification email to employee(s) who has related working
            var sendCc = string.Empty;
            var employees = this._employeeRepository.GetEmployeeForRelatedTeam(employee.Id);

            if (employees != null)
            {
                employees = employees.Where(i => i.Id != processUser.Id && i.Id != request.EmpId && !string.IsNullOrWhiteSpace(i.Email)).ToList();
                if (employees != null)
                {
                    sendCc = string.Join(",", employees.Select(t => t.Email));
                }
            }

            switch (request.RequestType)
            {
                case (short)RequestTypes.CheckIn:
                    var subjectCheckIn = string.Format(Messages.ApprovalNotificationCheckInSubject, employee.NickName, processUser.NickName);
                    CheckInOutRelatedMail(employee, processUser, request, subjectCheckIn, sendCc, templateEmailPath);
                    break;
                case (short)RequestTypes.CheckOut:
                    var subjectCheckOut = string.Format(Messages.ApprovalNotificationCheckOutSubject, employee.NickName, processUser.NickName);
                    CheckInOutRelatedMail(employee, processUser, request, subjectCheckOut, sendCc, templateEmailPath);
                    break;
                case (short)RequestTypes.LateIn:
                    var subjectLateIn = string.Format(Messages.ApprovalNotificationLateInSubject, employee.NickName, processUser.NickName);
                    LateInEarlyOutRelatedMail(employee, processUser, request, subjectLateIn, sendCc, true, templateEmailPath);
                    break;
                case (short)RequestTypes.EarlyOut:
                    var subjectEarlyOut = string.Format(Messages.ApprovalNotificationEarlyOutSubject, employee.NickName, processUser.NickName);
                    LateInEarlyOutRelatedMail(employee, processUser, request, subjectEarlyOut, sendCc, false, templateEmailPath);
                    break;
                case (short)RequestTypes.ChangeShift:
                    var subjectChangeShift = string.Format(Messages.ApprovalNotificationChangeShiftSubject, employee.NickName, processUser.NickName);
                    ChangeShiftRelatedMail(employee, processUser, request, subjectChangeShift, sendCc, templateEmailPath);
                    break;
                case (short)RequestTypes.Leave:
                    var subjectLeave = string.Format(Messages.ApprovalNotificationLeaveSubject, employee.NickName, processUser.NickName);
                    LeaveRelatedMail(employee, processUser, request, subjectLeave, sendCc, templateEmailPath);
                    break;
                case (short)RequestTypes.Overtime:
                    var subjectOvertime = string.Format(Messages.ApprovalNotificationOvertimeSubject, employee.NickName, processUser.NickName);
                    OvertimeRelatedMail(employee, processUser, request, subjectOvertime, sendCc, templateEmailPath);
                    break;
            }
        }

        /// <summary>
        /// Function to send mail check in out request to other staff which related group.
        /// </summary>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void CheckInOutRelatedMail(Employee employee, Employee processUser, WorkingTimeRequest request, string subject, string sendCc, string templateEmailPath)
        {
            var shift = this._shiftService.GetByDate(employee.Id, request.FromDate);
            var body = Utilities.ReadTextFile(templateEmailPath);
            var bodyContent = new
            {
                User = processUser.NickName,
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                Request = request.RequestType.ToText().ToLower(),
                ForDate = request.FromDate.ToDateMonthYearFormatedDate(),
                Time = HourFormat(request.FromDate),
                Shift = shift != null ? shift.Name : string.Empty,
                Url = this._baseUrl
            };

            body = RazorEngine.Razor.Parse(body, bodyContent);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                processUser.Email,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail late in, early out request to other staff which related group.
        /// </summary>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="isLateIn">Is late in or early out.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void LateInEarlyOutRelatedMail(
            Employee employee, 
            Employee processUser, 
            WorkingTimeRequest request, 
            string subject, 
            string sendCc, 
            bool isLateIn, 
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = GetStringDateForLateInEarlyOutRequest(request.FromDate, request.ToDate.Value);

            var bodyContent = new
            {
                User = processUser.NickName,
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                Request = request.RequestType == (short)RequestTypes.LateIn
                                ? "a " + Resources.Labels.LateIn.ToLower()
                                : "an " + Resources.Labels.EarlyOut.ToLower(),
                ForDate = forDate,
                Minute = isLateIn ? ("+" + request.LateInMins) : ("-" + request.EarlyOutMins),
                Remark = request.Remark,
                Url = this._baseUrl
            };

            body = RazorEngine.Razor.Parse(body, bodyContent);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                processUser.Email,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail change shift request to other staff which related group.
        /// </summary>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void ChangeShiftRelatedMail(
            Employee employee, 
            Employee processUser, 
            WorkingTimeRequest request, 
            string subject, 
            string sendCc, 
            string templateEmailPath)
        {
            var allShifts = this._shiftService.GetAll();
            var fromShift = allShifts.Where(t => t.Id == (request.FromShift.HasValue ? request.FromShift.Value : 0)).SingleOrDefault();
            var toShift = allShifts.Where(t => t.Id == (request.ToShift.HasValue ? request.ToShift.Value : 0)).SingleOrDefault();
            var body = Utilities.ReadTextFile(templateEmailPath);
            var bodyContent = new
            {
                User = processUser.NickName,
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                Request = request.RequestType.ToText().ToLower(),
                ForDate = request.FromDate.ToDateMonthYearFormatedDate(),
                FromShift = fromShift != null ? fromShift.Name : string.Empty,
                ToShift = toShift != null ? toShift.Name : string.Empty,
                Remark = request.Remark,
                Url = this._baseUrl
            };

            body = RazorEngine.Razor.Parse(body, bodyContent);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                processUser.Email,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail leave request to other staff which related group.
        /// </summary>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void LeaveRelatedMail(Employee employee, Employee processUser, WorkingTimeRequest request, string subject, string sendCc, string templateEmailPath)
        {
            var allLeaveTypes = _leaveTypeService.GetAll();
            var leaveType = allLeaveTypes.Where(t => t.Id == (request.LeaveTypeID.HasValue ? request.LeaveTypeID.Value : 0)).SingleOrDefault();
            var body = Utilities.ReadTextFile(templateEmailPath);

            var toDate = request.ToDate.HasValue ? request.ToDate.Value : request.FromDate;
            var forDate = GetStringDateForLeaveRequest(request.FromDate, toDate, request.TotalDays.HasValue ? request.TotalDays.Value : 0, request.TotalDays == 0);

            var bodyContent = new
            {
                User = processUser.NickName,
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                Request = request.RequestType.ToText().ToLower(),
                ForDate = forDate,
                Remark = request.Remark,
                LeaveType = leaveType != null ? leaveType.Name : string.Empty,
                Url = this._baseUrl
            };

            body = RazorEngine.Razor.Parse(body, bodyContent);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                processUser.Email,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail overtime request to other staff which related group.
        /// </summary>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void OvertimeRelatedMail(Employee employee, Employee processUser, WorkingTimeRequest request, string subject, string sendCc, string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var overtimeTypeId = request.OvertimeType.HasValue ? request.OvertimeType.Value : 0;
            var allShifts = this._shiftService.GetAll();
            var shift = allShifts.Where(t => t.Id == (request.FromShift.HasValue ? request.FromShift.Value : 0)).SingleOrDefault();

            var bodyContent = new
            {
                User = processUser.NickName,
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                Request = request.RequestType.ToText().ToLower(),
                ForDate = request.FromDate.ToDateMonthYearFormatedDate(),
                OvertimeType = new OvertimeType { Id = (short)overtimeTypeId }.Name,
                Details = request.IsOvertimeHours.Value
                    ? (request.TotalDays + "' (" + HourFormat(request.FromDate) + "-" + HourFormat(request.ToDate.Value) + ")")
                    : (shift != null ? shift.Name : string.Empty),
                Remark = request.Remark,
                Url = this._baseUrl
            };

            body = RazorEngine.Razor.Parse(body, bodyContent);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                processUser.Email,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to get string date for leave request.
        /// </summary>
        /// <param name="fromDate">The From date.</param>
        /// <param name="toDate">The To date.</param>
        /// <param name="totalDays">Total days.</param>
        /// <param name="isLeaveDuringWorkingHours">Is leave during working hours.</param>
        /// <returns>String detail date.</returns>
        private string GetStringDateForLeaveRequest(DateTime fromDate, DateTime toDate, decimal totalDays, bool isLeaveDuringWorkingHours)
        {
            var forDate = string.Empty;
            if (isLeaveDuringWorkingHours)
            {
                forDate = string.Format(_MailContentLeaveDuringWorking, fromDate.ToDateMonthYearFormatedDate(), HourFormat(fromDate), HourFormat(toDate));
            }
            else if (totalDays > 1)
            {
                forDate = string.Format(_MailContentLeave, fromDate.ToDateMonthYearFormatedDate(), toDate.ToDateMonthYearFormatedDate(), totalDays);
            }
            else
            {
                if (totalDays == 1)
                {
                    forDate = string.Format(_MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate());
                }
                else
                {
                    forDate = string.Format(_MailContentLeaveHalfDay, fromDate.ToDateMonthYearFormatedDate(), HourFormat(fromDate), HourFormat(toDate));
                }
            }

            return forDate;
        }

        /// <summary>
        /// Function to get string date for late in, early out request.
        /// </summary>
        /// <param name="fromDate">The From date.</param>
        /// <param name="toDate">The To date.</param>
        /// <returns>String detail date.</returns>
        private string GetStringDateForLateInEarlyOutRequest(DateTime fromDate, DateTime toDate)
        {
            var date = string.Empty;

            if (toDate == null || (toDate - fromDate).Days == 0)
            {
                date = fromDate.ToDateMonthYearFormatedDate();
            }
            else
            {
                date = "from " + fromDate.ToDateMonthYearFormatedDate() + " to " + toDate.ToDateMonthYearFormatedDate();
            }

            return date;
        }

        /// <summary>
        /// Function to send mail for leave request.
        /// </summary>
        /// <param name="model">LeaveRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is Update or insert.</param>
        /// <param name="templateEmailPath">Path of email template.</param>
        /// <param name="leaders">List Leader of requester.</param>
        private void SendMailForLeaveRequest(LeaveRequestViewModel model, long requestId, bool isUpdate, string templateEmailPath, IList<Employee> leaders)
        {
            // send email(s)
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            foreach (var item in leaders)
            {
                string forDate = GetStringDateForLeaveRequest(model.FromDate, model.ToDate, model.TotalDays, model.IsLeaveDuringWorking);
                string subject = (isUpdate ? Messages.UpdatedLeaveEmailSubject : Messages.LeaveEmailSubject) + " " + model.Employee;

                if (model.TotalDays > 1)
                {
                    subject += " from " + model.FromDate.ToDateMonthYearFormatedDate() + " to " + model.ToDate.ToDateMonthYearFormatedDate();
                }
                else
                {
                    subject += " on " + model.FromDate.ToDateMonthYearFormatedDate();
                }

                var content = new
                {
                    Leader = item.NickName,
                    Member = model.DisplayName,
                    RequestId = requestId,
                    ForDate = forDate,
                    Remark = model.Remark,
                    LeaveType = model.LeaveTypeName,
                    Url = this._baseUrl
                };

                var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                this._emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    item.Email,
                    string.Empty,
                    subject,
                    body);
            }
        }

        /// <summary>
        /// Function to send mail for change shift request.
        /// </summary>
        /// <param name="model">ChangeShift ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is Update or insert.</param>
        /// <param name="templateEmailPath">Path of email template.</param>
        /// <param name="leaders">List Leader of requester.</param>
        private void SendMailForChangeShiftRequest(ChangeShiftViewModel model, long requestId, bool isUpdate, string templateEmailPath, IList<Employee> leaders)
        {
            // send email(s)
            var subject = (isUpdate ? Messages.UpdatedChangeShiftEmailSubject : Messages.ChangeShiftEmailSubject)
                + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate();

            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            foreach (var item in leaders)
            {
                var content = new
                {
                    Leader = item.NickName,
                    Member = model.DisplayName,
                    RequestId = requestId,
                    ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                    FromShift = model.FromShiftName,
                    ToShift = model.ToShiftName,
                    Remark = model.Remark,
                    Url = this._baseUrl
                };
                var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                this._emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    item.Email,
                    string.Empty,
                    subject,
                    body);
            }
        }

        /// <summary>
        /// Function to send mail for late in, early out request.
        /// </summary>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is Update or insert.</param>
        /// <param name="templateEmailPath">Path of email template.</param>
        /// <param name="leaders">List Leader of requester.</param>
        private void SendMailForLateInEarlyOutRequest(
            LateInEarlyOutRequestViewModel model, 
            long requestId, 
            bool isUpdate, 
            string templateEmailPath, 
            IList<Employee> leaders)
        {
            // send email(s)
            var forDate = GetStringDateForLateInEarlyOutRequest(model.FromDate, model.ToDate);
            var subject = (isUpdate ? Messages.UpdatedLateInEarlyOutEmailSubject : Messages.LateInEarlyOutEmailSubject)
                + " " + model.Employee 
                + ((model.ToDate - model.FromDate).Days == 0 ? " on " + model.FromDate.ToDateMonthYearFormatedDate() : forDate);
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            foreach (var item in leaders)
            {
                var content = new
                {
                    Leader = item.NickName,
                    Member = model.DisplayName,
                    Type = model.Type == (short)RequestTypes.LateIn
                        ? "a " + Resources.Labels.LateIn.ToLower()
                        : "an " + Resources.Labels.EarlyOut.ToLower(),
                    RequestId = requestId,
                    ForDate = forDate,
                    Minute = model.Minute,
                    Remark = model.Remark,
                    Url = this._baseUrl
                };

                var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                this._emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    item.Email,
                    string.Empty,
                    subject,
                    body);
            }
        }

        /// <summary>
        /// Function to send mail for check in out request.
        /// </summary>
        /// <param name="model">CheckInOutRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is Update or insert.</param>
        /// <param name="templateEmailPath">Path of email template.</param>
        /// <param name="leaders">List Leader of requester.</param>
        private void SendMailForCheckInOutRequest(CheckInOutRequestViewModel model, long requestId, bool isUpdate, string templateEmailPath, IList<Employee> leaders)
        {
            // send email(s)
            var subject = (isUpdate ? Messages.UpdatedCheckInOutEmailSubject : Messages.CheckInOutEmailSubject)
                + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate();

            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);

            foreach (var item in leaders)
            {
                var content = new
                {
                    Leader = item.NickName,
                    Member = model.DisplayName,
                    Type = model.Type == (short)RequestTypes.CheckIn
                        ? RequestTypes.CheckIn.EnumToString().ToLower()
                        : RequestTypes.CheckOut.EnumToString().ToLower(),
                    RequestId = requestId,
                    ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                    Time = model.Hour.ToString().PadLeft(2, '0') + ":" + model.Minute.ToString().PadLeft(2, '0'),
                    Shift = model.Shift,
                    Url = this._baseUrl
                };
                var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                this._emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    item.Email,
                    string.Empty,
                    subject,
                    body);
            }
        }

        /// <summary>
        /// Function to send mail for overtime request.
        /// </summary>
        /// <param name="model">OvertimeRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is Update or insert.</param>
        /// <param name="templateEmailPath">Path of email template.</param>
        /// <param name="leaders">List Leader of requester.</param>
        private void SendMailForOvertimeRequest(OvertimeRequestViewModel model, long requestId, bool isUpdate, string templateEmailPath, IList<Employee> leaders)
        {
            // send email(s)
            var subject = (isUpdate ? Messages.UpdatedOvertimeEmailSubject : Messages.OvertimeEmailSubject)
                + " " + model.Employee + " on " + model.Date.ToDateMonthYearFormatedDate();
            var allShifts = this._shiftService.GetAll();
            var shift = allShifts.Where(t => t.Id == model.Shift).SingleOrDefault();
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            foreach (var item in leaders)
            {
                var content = new
                {
                    Leader = item.NickName,
                    Member = model.DisplayName,
                    OvertimeType = new OvertimeType { Id = model.OvertimeType }.Name,
                    ForDate = model.Date.ToDateMonthYearFormatedDate(),
                    Details = model.IsOvertimeHours 
                    ? (model.TotalMinutes + "' (" + model.FromTime + "-" + model.ToTime + ")") 
                    : (shift != null ? shift.Name : string.Empty),
                    Remark = model.Remark,
                    Url = this._baseUrl
                };
                var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                this._emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    item.Email,
                    string.Empty,
                    subject,
                    body);
            }
        }

        /// <summary>
        /// Function to get path of email template.
        /// </summary>
        /// <param name="requestType">Request type.</param>
        /// <returns>Path of email template.</returns>
        private string GetEmailTemplatePath(short requestType)
        {
            var path = string.Empty;
            switch (requestType)
            {
                case (short)RequestTypes.CheckIn:
                    path = HttpContext.Current.Server.MapPath("~/app_data/emails/CheckInOutApprovalNotification.cshtml");
                    break;
                case (short)RequestTypes.CheckOut:
                    path = HttpContext.Current.Server.MapPath("~/app_data/emails/CheckInOutApprovalNotification.cshtml");
                    break;
                case (short)RequestTypes.LateIn:
                    path = HttpContext.Current.Server.MapPath("~/app_data/emails/LateInEarlyOutApprovalNotification.cshtml");
                    break;
                case (short)RequestTypes.EarlyOut:
                    path = HttpContext.Current.Server.MapPath("~/app_data/emails/LateInEarlyOutApprovalNotification.cshtml");
                    break;
                case (short)RequestTypes.ChangeShift:
                    path = HttpContext.Current.Server.MapPath("~/app_data/emails/ChangeShiftApprovalNotification.cshtml");
                    break;
                case (short)RequestTypes.Leave:
                    path = HttpContext.Current.Server.MapPath("~/app_data/emails/LeaveApprovalNotification.cshtml");
                    break;
                case (short)RequestTypes.Overtime:
                    path = HttpContext.Current.Server.MapPath("~/app_data/emails/OvertimeApprovalNotification.cshtml");
                    break;
            }

            return path;
        }

        /// <summary>
        /// Function to send mail for saved success for late in, early out request.
        /// </summary>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is update or insert.</param>
        private void SendMailForLateInEarlyOutSuccess(LateInEarlyOutRequestViewModel model, long requestId, bool isUpdate)
        {
            var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);
            if (leaders == null || leaders.Count == 0)
            {
                string errorMessage;
                this.Process(Commands.Approve.EnumToString(), requestId, model.EmployeeId, out errorMessage);
            }
            else
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath("~/app_data/emails/LateInEarlyOut.Request.cshtml");
                var taskSendMail = new Task(() => this.SendMailForLateInEarlyOutRequest(model, requestId, isUpdate, templateEmailPath, leaders));
                taskSendMail.Start();
            }
        }

        /// <summary>
        /// Function to send mail for saved success for leave request.
        /// </summary>
        /// <param name="model">LeaveRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is update or insert.</param>
        private void SendMailForLeaveSuccess(LeaveRequestViewModel model, long requestId, bool isUpdate)
        {
            var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

            if (leaders == null || leaders.Count == 0)
            {
                string errorMessage;
                this.Process(Commands.Approve.EnumToString(), model.Id, model.EmployeeId, out errorMessage);
            }
            else
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath("~/app_data/emails/Leave.Request.cshtml");
                var taskSendMail = new Task(() => this.SendMailForLeaveRequest(model, model.Id, isUpdate, templateEmailPath, leaders));
                taskSendMail.Start();
            }
        }

        /// <summary>
        /// Function to send mail for saved success for Overtime request.
        /// </summary>
        /// <param name="model">OvertimeRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is update or insert.</param>
        private void SendMailForOvertimeSuccess(OvertimeRequestViewModel model, long requestId, bool isUpdate)
        {
            var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);
            if (leaders == null || leaders.Count == 0)
            {
                string errorMessage;
                this.Process(Commands.Approve.EnumToString(), requestId, model.EmployeeId, out errorMessage);
            }
            else
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath("~/app_data/emails/Overtime.Request.cshtml");
                var taskSendMail = new Task(() => this.SendMailForOvertimeRequest(model, requestId, false, templateEmailPath, leaders));
                taskSendMail.Start();
            }
        }

        /// <summary>
        /// Function to check is valid of late in, early out request object.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <returns>Is valid or no.</returns>
        private bool CheckIsValidLateInEarlyOutRequest(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model)
        {
            var isValid = true;
            if (modelState.IsValid)
            {
                if (model.ToDate >= model.FromDate)
                {
                    if (model.Minute <= 0 || model.Minute > _MaxMunites)
                    {
                        modelState.AddModelError(string.Empty, Resources.Messages.WrongMunites);
                        isValid = false;
                    }
                }
                else
                {
                    modelState.AddModelError(string.Empty, Resources.Messages.WrongDateRange);
                    isValid = false;
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Function to check is valid of leave request object.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">LeaveRequest ViewModel.</param>
        /// <returns>Is valid or no.</returns>
        private bool CheckIsValidLeaveRequest(ModelStateDictionary modelState, LeaveRequestViewModel model)
        {
            bool isValid = true;
            if (model.HalfDayOff)
            {
                model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                if (model.TotalDays != (decimal)0.5)
                {
                    modelState.AddModelError("TotalDays", Messages.InvalidTotalDays);
                    isValid = false;
                }
            }
            else if (model.TotalDays % 1 != 0)
            {
                modelState.AddModelError("TotalDays", Messages.InvalidTotalDays);
                isValid = false;
            }

            return isValid;
        }
    }
}