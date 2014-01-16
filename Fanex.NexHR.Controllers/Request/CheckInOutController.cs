// --------------------------------------------------------------------------
// <copyright file="CheckInOutController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Apply StyleCop</change>
// </history>
namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// CheckInOut Controller.
    /// </summary>
    [Authorize]
    public class CheckInOutController : RequestController
    {
        #region Ctor

        public CheckInOutController()
            : base(null, null, null, null)
        {
        }

        #endregion Ctor

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        public ActionResult Create()
        {
            var listUnavailDates = _EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;
            var employee = _EmployeeService.Get(this.CurrentUser.Id);
            ViewBag.Shifts = _ShiftService.GetAll();
            var viewModel = new CheckInOutRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                ForDate = DateTime.Today,
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute,
                Type = (short)RequestTypes.CheckIn,
                By = this.CurrentUser.Id
            };
            var shift = _ShiftService.GetByDate(employee.Id, DateTime.Now);
            if (shift != null)
            {
                viewModel.Hour = int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[0]);
                viewModel.Minute = int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[1]);
            }

            /*Anders check null shiftdate*/
            viewModel.Shift = (shift == null) ? string.Empty : shift.Name;

            // Count number of Check in/out request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            IList<RequestEntity> entityList = _RequestService.GetCheckInOutRequest(
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

            return View(viewModel);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">CheckInOutRequest ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(CheckInOutRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Shifts = _ShiftService.GetAll();
            if (string.Compare(model.Employee, model.DisplayName, true) != 0)
            {
                model.DisplayName = model.Employee;
            }

            ViewBag.Requested = _RequestService.CheckInOut(ModelState, model);
            ViewBag.Year = model.ForDate.Year;

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

            ViewBag.Shifts = _ShiftService.GetAll();

            var request = _RequestService.GetWorkingTimeRequest(id);

            if (!this.Editable(request.Status, request.EmpId, request.CreatedBy))
            {
                return RedirectToAction("EmployeeView", "CheckInOut");
            }

            var employee = _EmployeeService.Get(request.EmpId);
            var shift = _ShiftService.GetByDate(employee.Id, DateTime.Now);
            return View(new CheckInOutRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                ForDate = request.FromDate.Date,
                Hour = request.FromDate.Hour,
                Minute = request.FromDate.Minute,
                Type = request.RequestType,
                By = this.CurrentUser.Id,
                Shift = (shift == null) ? string.Empty : shift.Name
            });
        }

        [HttpPost]
        public ActionResult Update(CheckInOutRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Shifts = _ShiftService.GetAll();
            ViewBag.Updated = _RequestService.UpdateCheckInOut(ModelState, model);
            ViewBag.Year = model.ForDate.Year;
            return View(model);
        }

        public ActionResult Delete()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// Get check in/out request of a specific employee in a year.
        /// </summary>
        /// <param name="year">Current Year.</param>
        /// <returns>View Employee.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<RequestEntity> entityList = _RequestService.GetCheckInOutRequest(
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
