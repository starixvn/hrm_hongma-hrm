// --------------------------------------------------------------------------
// <copyright file="OvertimeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.3">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Overtime Controller.
    /// </summary>
    public class OvertimeController : RequestController
    {
        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="year">Inputted Year.</param>
        /// <returns>View Employee's Overtime Request.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<RequestEntity> entityList = _RequestService.GetOvertimeRequest(
                (short)RolesEnum.Member,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                string.Empty,
                true,
                this.CurrentUser.Id);

            return View(entityList);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        public ActionResult Create()
        {
            var employee = _EmployeeService.Get(this.CurrentUser.Id);

            var model = new OvertimeRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                Date = DateTime.Now.Date,
                TotalMinutes = 0,
                By = this.CurrentUser.Id
            };

            // Count number days of Leave request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            IList<RequestEntity> entityList = _RequestService.GetOvertimeRequest(
                (short)RolesEnum.Member,
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                string.Empty,
                true,
                this.CurrentUser.Id);
            foreach (var request in entityList)
            {
                if (request.Status.Id != (byte)RequestStatuses.Reject)
                {
                    numberOfRequest++;
                }
            }

            ViewBag.TotalRequest = numberOfRequest;
            ViewBag.Shifts = _ShiftService.GetAll();
            return View(model);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">OvertimeRequest ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(OvertimeRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Year = model.Date.Year;
            if (string.Compare(model.Employee, model.DisplayName, true) != 0)
            {
                model.DisplayName = model.Employee;
            }

            ViewBag.Requested = _RequestService.Overtime(ModelState, model);
            ViewBag.Shifts = _ShiftService.GetAll();
            ViewBag.Year = model.Date.Year;
            return View(model);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">WorkingTimeRequest Id.</param>
        /// <returns>View Update.</returns>
        public ActionResult Update(int id)
        {
            var request = _RequestService.GetWorkingTimeRequest(id);
            var employee = _EmployeeService.Get(request.EmpId);
            var model = new OvertimeRequestViewModel
            {
                Id = request.Id,
                OvertimeType = request.OvertimeType.Value,
                IsOvertimeHours = request.IsOvertimeHours.Value,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                Shift = request.FromShift.Value,
                Date = request.FromDate.Date,
                FromTime = Utilities.GetTime(request.FromDate),
                ToTime = Utilities.GetTime(request.ToDate),
                IsCrossDay = request.ToDate.Value.Date.AddDays(-1) == request.FromDate.Date,
                TotalMinutes = (int)request.TotalDays,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark,
                By = this.CurrentUser.Id
            };

            ViewBag.Shifts = _ShiftService.GetAll();
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(OvertimeRequestViewModel model)
        {
            ViewBag.Shifts = _ShiftService.GetAll();
            ViewBag.Updated = _RequestService.UpdateOvertime(ModelState, model);
            return View(model);
        }
    }
}
