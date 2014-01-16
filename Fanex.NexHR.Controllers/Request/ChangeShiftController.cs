// --------------------------------------------------------------------------
// <copyright file="ChangeShiftController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// ChangeShift Controller.
    /// </summary>
    [Authorize]
    public class ChangeShiftController : RequestController
    {
        #region Ctor
        public ChangeShiftController()
            : base(null, null, null, null)
        { 
        }
        #endregion

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        public ActionResult Create()
        {
            var listUnavailDates = _EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;
            var employee = _EmployeeService.Get(this.CurrentUser.Id);
            var shift = _ShiftService.GetByDate(employee.Id, DateTime.Now.Date);
            var model = new ChangeShiftViewModel
            {
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                ForDate = DateTime.Now.Date,
                DisplayName = employee.DisplayName,
                By = this.CurrentUser.Id
            };
            if (shift != null)
            {
                model.FromShift = (int)shift.Id;
                model.FromShiftName = shift.Name;
            }

            // Count number of ChangeShift request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            IList<RequestEntity> entityList = _RequestService.GetChangeShiftRequest(
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
        /// <param name="model">ChangeShift ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(ChangeShiftViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Shifts = _ShiftService.GetAll();
            if (string.Compare(model.Employee, model.DisplayName, true) != 0)
            {
                model.DisplayName = model.Employee;
            }

            ViewBag.Requested = _RequestService.ChangeShift(ModelState, model);
            return View(model);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">Working time request's id.</param>
        /// <returns>View update.</returns>
        public ActionResult Update(int id)
        {
            var listUnavailDates = _EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;
            var request = _RequestService.GetWorkingTimeRequest(id);
            var shift = _ShiftService.GetByDate(this.CurrentUser.Id, request.FromDate.Date);
            var employee = _EmployeeService.Get(request.EmpId);
            var model = new ChangeShiftViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                Id = request.Id,
                ForDate = request.FromDate.Date,
                FromShift = (int)shift.Id,
                FromShiftName = shift.Name,
                ToShift = (int)request.ToShift,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark,
                By = this.CurrentUser.Id
            };

            ViewBag.Shifts = _ShiftService.GetAll();
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(ChangeShiftViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Shifts = _ShiftService.GetAll();
            ViewBag.Updated = _RequestService.UpdateChangeShift(ModelState, model);
            return View(model);
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="year">Input year.</param>
        /// <returns>View Employee according to year.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<RequestEntity> entityList = _RequestService.GetChangeShiftRequest(
                (short)RolesEnum.Member,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                string.Empty,
                true,
                this.CurrentUser.Id);

            return View(entityList);
        }
    }
}
