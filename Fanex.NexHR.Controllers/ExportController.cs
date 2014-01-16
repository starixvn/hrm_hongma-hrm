// --------------------------------------------------------------------------
// <copyright file="ExportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using Fanex.NexHR.Controllers.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// Export Controller.
    /// </summary>
    public class ExportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly IExportExcelService _exportService;
        private readonly IRequestService _requestService;
        private readonly ITeamService _teamService;

        public ExportController(
            IReportService reportService = null,
            IExportExcelService exportService = null,
            IRequestService requestService = null,
            ITeamService teamService = null)
        {
            this._reportService = reportService ?? new ReportService();
            this._exportService = exportService ?? new ExportExcelService();
            this._requestService = requestService ?? new RequestService();
            this._teamService = teamService ?? new TeamService();
        }

        public ExportController()
            : this(null, null, null, null)
        {
        }

        /// <summary>
        /// Export excel Total Working Hours.
        /// </summary>
        /// <param name="model">TotalWorkingHours ViewModel.</param>
        /// <returns>ActionResult Total Working Hours.</returns>
        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            if (model == null)
            {
                model = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
                };
            }

            string fileName = "TotalWorkingHours_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            IList<TotalWorkingHours> listTotalWorkingHours = _reportService.GetWorkingHoursList(
                model.FromDate,
                model.ToDate,
                empId: (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                groupIds: model.TeamIds);

            var hssfworkbook = _exportService.ExportTotalWorkingHours(listTotalWorkingHours, model.FromDate, model.ToDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// Export excel Attendance report.
        /// </summary>
        /// <param name="model">EmployeeAttendanceViewModel object.</param>
        /// <returns>ActionResult Attendance report.</returns>
        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            string fileName = "AttendanceReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var employeeAttendance = _reportService.GetEmployeeAttendanceReport(
                    model.FromDate,
                    model.ToDate,
                    model.Type,
                    model.EmpId,
                    model.GroupIds,
                    model.ExcludeUserDelete ? 1 : 0);

            var hssfworkbook = _exportService.ExportAttendance(employeeAttendance, model.FromDate, model.ToDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// ActionResult LeaveSummary.
        /// </summary>
        /// <param name="model">LeaveSummary ViewModel.</param>
        /// <returns>View LeaveSummary.</returns>
        public ActionResult LeaveSummary(LeaveSummaryViewModel model)
        {
            string fileName = "LeaveSummary_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            IList<LeaveSummary> leaveSummary = _reportService.GetAnnualLeaveBalanceReport(
                model.Year,
                empId: model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                groupIds: model.TeamIds);

            var teams = _teamService.GetAll();
            var leaveSummaryByTeamList = _reportService.GetAnnualLeaveByTeam(leaveSummary, teams);

            var hssfworkbook = _exportService.ExportLeaveSummary(leaveSummaryByTeamList, model.Year);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// ActionResult ViewWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Working Time Request.</returns>
        public ActionResult ViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var requestList = _requestService.GetWorkingTimeRequests(
                model,
                (short)RolesEnum.HR,
                false,
                null);

            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// ActionResult MemberWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Member working time request.</returns>
        [Authorize(Roles = "AssistantLeader")]
        public ActionResult MemberWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var requestList = _requestService.GetWorkingTimeRequests(
                model,
                (short)RolesEnum.AssistantLeader,
                false,
                new int?(this.CurrentUser.Id));

            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        ///  ActionResult Human Resource ViewWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Working Time Request.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HRViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var requestList = _requestService.GetWorkingTimeRequests(
                model,
                (short)RolesEnum.HR,
                false,
                null);

            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// ActionResult EmployeeViewWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Employee Working Time Request.</returns>
        public ActionResult EmployeeViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var requestList = _requestService.GetWorkingTimeRequests(
                model,
                (short)RolesEnum.HR,
                false,
                null);

            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// ActionResult DailyViewWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Daily Working Time Request.</returns>
        public ActionResult DailyViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var requestList = _requestService.GetWorkingTimeRequests(
                model,
                (short)RolesEnum.HR,
                true,
                null);

            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);

            return ExcelFile(hssfworkbook, fileName);
        }

        private ActionResult ExcelFile(HSSFWorkbook hssfworkbook, string fileName)
        {
            MemoryStream output = new MemoryStream();
            hssfworkbook.Write(output);

            // Return the result to the end user
            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}
