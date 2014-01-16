// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutController.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// LateInEarlyOut Controller.
    /// </summary>
    [Authorize]
    public class LateInEarlyOutController : RequestController
    {
        #region Ctor

        public LateInEarlyOutController()
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

            //// Count number of Late in/early out request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            IList<RequestEntity> entityList = _RequestService.GetLateInEarlyOutRequest(
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

            return View(new LateInEarlyOutRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                FromTime = "08:00",
                ToTime = "09:00",
                Minute = 60,
                Type = (short)RequestTypes.LateIn,
                By = this.CurrentUser.Id
            });
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(LateInEarlyOutRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            if (string.Compare(model.Employee, model.DisplayName, true) != 0)
            {
                model.DisplayName = model.Employee;
            }

            ViewBag.Requested = _RequestService.LateInEarlyOut(ModelState, model);
            ViewBag.Year = model.FromDate.Year;
            return View(model);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">WorkingTimeRequest id.</param>
        /// <returns>View Update.</returns>
        public ActionResult Update(int id)
        {
            var listUnavailDates = _EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;

            var request = _RequestService.GetWorkingTimeRequest(id);
            var employee = _EmployeeService.Get(request.EmpId);
            int minute = request.RequestType == (short)RequestTypes.LateIn
                ? (int)request.LateInMins
                : (int)request.EarlyOutMins;

            return View(new LateInEarlyOutRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Value.Date,
                FromTime = request.FromDate.ToString("HH:mm"),
                ToTime = request.ToDate.Value.ToString("HH:mm"),
                Minute = minute,
                Type = request.RequestType,
                By = this.CurrentUser.Id,
                Remark = request.Remark
            });
        }

        [HttpPost]
        public ActionResult Update(LateInEarlyOutRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Updated = _RequestService.UpdateLateInEarlyOut(ModelState, model);
            ViewBag.Year = model.FromDate.Year;
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
        /// <param name="year">Inputted Year.</param>
        /// <returns>View GetLateInEarlyOut of Employee in the Year.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<RequestEntity> entityList = _RequestService.GetLateInEarlyOutRequest(
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
