// --------------------------------------------------------------------------
// <copyright file="ReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report Controller.
    /// </summary>
    [Authorize]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly ITeamService _teamService;

        public ReportController(
            IReportService reportSevice = null,
            ITeamService teamService = null)
        {
            this._reportService = reportSevice ?? new ReportService();
            this._teamService = teamService ?? new TeamService();
        }

        public ReportController()
            : this(null, null)
        {
        }

        /// <summary>
        /// ActionResult Attendance.
        /// </summary>
        /// <param name="model">EmployeeAttendance ViewModel.</param>
        /// <returns>View EmployeeAttendance.</returns>
        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            var employee = this.CurrentUser;
            ViewBag.Groups = _teamService.GetAll();

            if (model.FromDate == DateTime.MinValue)
            {
                model = new EmployeeAttendanceViewModel
                {
                    FromDate = DateTime.Now,
                    ToDate = DateTime.Now,
                    GroupIds = string.Empty,
                    DateOption = 0,
                    ExcludeUserDelete = true,
                };
            }

            return View(model);
        }

        public ActionResult Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTime.Now.Year,
                EmpId = CurrentUser.Id
            };
            return View(model);
        }

        /// <summary>
        /// Employee request monitoring in current month.
        /// </summary>
        /// <returns>Return View.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HrRequestMonitoring()
        {
            return HrRequestMonitoring(null);
        }

        /// <summary>
        /// ActionResult HumanResourceRequestMonitoring.
        /// </summary>
        /// <param name="model">RequestsMonitoring ViewModel.</param>
        /// <returns>View Human resource RequestMonitoring.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult HrRequestMonitoring(RequestsMonitoringViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new RequestsMonitoringViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    MaxRequest = 2,
                    DateOption = 0
                };
            }

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = false;

            ViewBag.EntityList = new List<RequestsMonitoring>();
            if (this.CurrentUser.Roles != null)
            {
                if (this.CurrentUser.Roles == ((int)RolesEnum.AssistantLeader).ToString()
                    || this.CurrentUser.Roles == ((int)RolesEnum.Leader).ToString()
                    || this.CurrentUser.Roles == ((int)RolesEnum.HR).ToString()
                    || this.CurrentUser.Roles == ((int)RolesEnum.SA).ToString())
                {
                    ViewBag.EntityList = _reportService.GetRequestMonitoring(model.FromDate, model.ToDate, model.MaxRequest, (int)RolesEnum.HR, this.CurrentUser.Id);
                }
            }

            return View("RequestMonitoring", model);
        }

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult LeaderRequestMonitoring()
        {
            return LeaderRequestMonitoring(null);
        }

        /// <summary>
        /// ActionResult LeaderRequestMonitoring.
        /// </summary>
        /// <param name="model">RequestsMonitoring ViewModel.</param>
        /// <returns>View LeaderRequestMonitoring.</returns>
        [HttpPost]
        [Authorize(Roles = "AssistantLeader")]
        public ActionResult LeaderRequestMonitoring(RequestsMonitoringViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new RequestsMonitoringViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    MaxRequest = 1,
                    DateOption = 0
                };
            }

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = true;

            ViewBag.EntityList = new List<RequestsMonitoring>();
            if (this.CurrentUser.Roles != null)
            {
                if (this.CurrentUser.Roles == ((int)RolesEnum.AssistantLeader).ToString()
                    || this.CurrentUser.Roles == ((int)RolesEnum.Leader).ToString()
                    || this.CurrentUser.Roles == ((int)RolesEnum.HR).ToString()
                    || this.CurrentUser.Roles == ((int)RolesEnum.SA).ToString())
                {
                    ViewBag.EntityList = _reportService.GetRequestMonitoring(model.FromDate, model.ToDate, model.MaxRequest, (int)RolesEnum.AssistantLeader, this.CurrentUser.Id);
                }
            }

            return View("RequestMonitoring", model);
        }

        /// <summary>
        /// ActionResult TotalWorkingHours.
        /// </summary>
        /// <param name="teamIds">Team's id.</param>
        /// <param name="employee">Employee name.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Count to Date.</param>
        /// <param name="dateOption">Date Options.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="isExportExcel">Is exported to Excel.</param>
        /// <returns>View TotalWorkingHours.</returns>
        public ActionResult TotalWorkingHours(
            string teamIds,
            string employee,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? dateOption = null,
            int? employeeId = null,
            bool isExportExcel = false)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var model = new TotalWorkingHoursViewModel 
            { 
                TeamIds = teamIds == null ? string.Empty : teamIds,
                Employee = employee,
                FromDate = fromDate.HasValue ? fromDate.Value : new DateTime(year, month, 1),
                ToDate = toDate.HasValue ? toDate.Value : new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                IsExportExcel = isExportExcel,
                DateOption = dateOption.HasValue ? dateOption.Value : 5
            };

            return TotalWorkingHours(model);
        }

        /// <summary>
        /// ActionResult TotalWorkingHours.
        /// </summary>
        /// <param name="model">TotalWorkingHours ViewModel.</param>
        /// <returns>View of TotalWorkingHours.</returns>
        [HttpPost]
        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
                    IsExportExcel = false
                };
            }

            if (model.IsExportExcel)
            {
                ExportController export = new ExportController();
                return export.TotalWorkingHours(model);
            }

            ViewBag.Groups = _teamService.GetAll();
            ViewBag.TeamIds = model.TeamIds;
            var totalWorkingHours = _reportService.GetWorkingHoursList(
                model.FromDate,
                model.ToDate,
                empId: (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                groupIds: model.TeamIds);

            ViewBag.EntityList = totalWorkingHours;

            return View(model);
        }

        /// <summary>
        /// ActionResult LeaveSummary.
        /// </summary>
        /// <param name="teamIds">Team's id.</param>
        /// <param name="employee">Employee name.</param>
        /// <param name="year">Current Year.</param>
        /// <param name="employeeId">Employee's Id.</param>
        /// <param name="minAnnualBalance">Min Annual Balance.</param>
        /// <returns>View LeaveSummary.</returns>
        public ActionResult LeaveSummary(
            string teamIds,
            string employee,
            int? year = null, 
            int? employeeId = null, 
            int? minAnnualBalance = null)
        {
            var model = new LeaveSummaryViewModel 
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year,
                TeamIds = teamIds,
                Employee = employee,
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                MinAnnualBalance = minAnnualBalance.HasValue ? minAnnualBalance.Value : 0
            };

            return LeaveSummary(model);
        }

        /// <summary>
        /// ActionResult LeaveSummary.
        /// </summary>
        /// <param name="model">LeaveSummary ViewModel.</param>
        /// <returns>View LeaveSummary.</returns>
        [HttpPost]
        public ActionResult LeaveSummary(LeaveSummaryViewModel model)
        {
            if (model == null)
            {
                model = new LeaveSummaryViewModel
                {
                    Year = DateTime.Now.Year,
                    EmployeeId = -1
                };
            }

            IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(
                model.Year,
                empId: model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                groupIds: model.TeamIds);

            IList<Team> teams = new List<Team>();
            teams = _teamService.GetAll();

            ViewBag.Groups = teams;
            ViewBag.TeamIds = model.TeamIds;
            ViewBag.EntityList = entityList;

            var leaveSummaryByTeamList = _reportService.GetAnnualLeaveByTeam(entityList, teams);

            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        public ActionResult ExtensionNo()
        {
            return View();
        }
    }
}
