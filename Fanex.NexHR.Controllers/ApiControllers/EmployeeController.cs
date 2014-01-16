// --------------------------------------------------------------------------
// <copyright file="EmployeeController.cs" company="Nexcel Solutions Vietnam">
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
    using System.Linq;
    using System.Web.Http;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Employee Controller API.
    /// </summary>
    public class EmployeeController : ApiController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController()
            : this(null)
        {
        }

        public EmployeeController(IEmployeeService employeeService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        public IQueryable<Employee> GetEmployees(string query, string groupIds = "", bool? enable = null, bool? isDelete = null, int? titleId = null)
        {
            if (groupIds == null)
            {
                groupIds = string.Empty;
            }

            var entityList = _employeeService.Query(query, groupIds, enable, isDelete, titleId).AsQueryable();

            return entityList;
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id)
        {
            return _employeeService.GetWorkingSchedule(date, id);
        }

        public int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return _employeeService.GetRangeDate(employeeId, fromDate, toDate);
        }

        public IEnumerable<DateTime> GetUnavailDate(int employeeId, DateTime date)
        {
            return _employeeService.GetDisableRequestDate(
                employeeId, 
                new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1), 
                new DateTime(date.AddMonths(2).Year, date.AddMonths(2).Month, 1).AddDays(-1));
        }
    }
}
