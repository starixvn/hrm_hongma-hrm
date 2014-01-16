// --------------------------------------------------------------------------
// <copyright file="LeaveController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Leave Controller.
    /// </summary>
    public class LeaveController : RequestController
    {
        #region Ctor
        public LeaveController()
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
            var employee = _EmployeeService.Get(this.CurrentUser.Id);

            var listUnavailDates = _EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;
            ViewBag.LeaveTypes = _LeaveTypeService.GetAll();
            var model = new LeaveRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                By = this.CurrentUser.Id
            };

            // Count number days of Leave request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            decimal numberDaysOfRequest = 0;
            IList<RequestEntity> entityList = _RequestService.GetLeaveRequest(
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
                    numberDaysOfRequest += request.TotalDays;
                }
            }

            ViewBag.TotalRequest = numberDaysOfRequest;
            return View(model);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">LeaveRequest ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(LeaveRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.LeaveTypes = _LeaveTypeService.GetAll();
            ViewBag.Year = model.FromDate.Year;
            if (string.Compare(model.Employee, model.DisplayName, true) != 0)
            {
                model.DisplayName = model.Employee;
            }

            ViewBag.Requested = _RequestService.Leave(ModelState, model);
            return View(model);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">WorkingTimeRequest id.</param>
        /// <returns>View update.</returns>
        public ActionResult Update(int id)
        {
            var listUnavailDates = _EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;

            var request = _RequestService.GetWorkingTimeRequest(id);
            var employee = _EmployeeService.Get(request.EmpId);
            var leaveType = _LeaveTypeService.GetLeaveType(request.LeaveTypeID.Value);

            var model = new LeaveRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = request.FromDate.Date,
                FromTime = Utilities.GetTime(request.FromDate),
                ToDate = request.ToDate.Value.Date,
                ToTime = Utilities.GetTime(request.ToDate),
                TotalDays = (decimal)request.TotalDays,
                LeaveTypeId = (int)request.LeaveTypeID,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark,
                By = this.CurrentUser.Id,
                HalfDayOff = request.TotalDays == (decimal)0.5,
                IsLeaveDuringWorking = leaveType.IsLeaveDuringWorking
            };

            ViewBag.LeaveTypes = _LeaveTypeService.GetAll();
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(LeaveRequestViewModel model)
        {
            ViewBag.LeaveTypes = _LeaveTypeService.GetAll();
            ViewBag.Updated = _RequestService.UpdateLeave(ModelState, model);
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
        /// <param name="year">Inputted year.</param>
        /// <returns>View Employee's Leave Request.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;
            //// kos push
            IList<RequestEntity> entityList = _RequestService.GetLeaveRequest(
                (short)RolesEnum.Member,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                string.Empty,
                true,
                this.CurrentUser.Id);

            return View(entityList);
        }

        public int GetLeaveType(int id)
        {
            return _LeaveTypeService.GetLeaveType(id).IsLeaveDuringWorking ? 1 : 0;
        }
    }
}
