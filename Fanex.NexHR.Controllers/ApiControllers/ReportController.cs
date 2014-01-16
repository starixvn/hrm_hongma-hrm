// --------------------------------------------------------------------------
// <copyright file="ReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Http;
    using Fanex.NexHR.Controllers.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report Controller API.
    /// </summary>
    public class ReportController : ApiController
    {
        private readonly IReportService _reportService;

        public ReportController()
        {
            _reportService = new ReportService();
        }

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Function to get list employee attendance.
        /// </summary>
        /// <param name="model">EmployeeAttendance ViewModel.</param>
        /// <returns>EmployeeAttendanceModel List.</returns>
        public IList<EmployeeAttendanceModel> GetEmployeeAttendances([FromUri]EmployeeAttendanceViewModel model)
        {
            if (model.GroupIds == null)
            {
                model.GroupIds = string.Empty;
            }

            var employeeAttendance = _reportService.GetEmployeeAttendanceReport(
                model.FromDate, 
                model.ToDate, 
                model.Type, 
                model.EmpId, 
                model.GroupIds, 
                model.ExcludeUserDelete ? 1 : 0);

            return employeeAttendance;
        }

        [HttpGet]
        [HttpPost]
        public IList<EmployeeAttendanceModel> Timesheet([FromUri]TimesheetViewModel model)
        {
            var fromDate = new DateTime(model.Year, model.Month, 1);
            var toDate = new DateTime(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month));
            return _reportService.GetEmployeeAttendanceReport(fromDate, toDate, model.Type, model.EmpId);
        }
    }
}
