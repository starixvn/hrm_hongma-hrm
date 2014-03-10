// --------------------------------------------------------------------------
// <copyright file="ExtensionNoController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.10">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// ExtensionNo Controller.
    /// </summary>
    public class ExtensionNoController : BaseController
    {
        private readonly IReportService _reportService;

        public ExtensionNoController(IReportService reportSevice = null)
        {
            this._reportService = reportSevice ?? new ReportService();
        }

        public ExtensionNoController()
            : this(null)
        {
        }

        /// <summary>
        /// Extension Number Index.
        /// </summary>
        /// <returns>View ExtensionNo.</returns>
        public ActionResult Index()
        {
            var lstEntity = _reportService.GetAllExtensionNo();
            var model = (from item in lstEntity
                         group item by item.TeamName into g
                         select new ExtensionNumberViewModel() { Name = g.Key, ListExt = g.ToList() }).ToList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            if (ModelState.IsValid)
            {
                ViewBag.Requested = _reportService.AddNewExtensionNoMember(model);
            }

            return View();
        }

        [Authorize(Roles = "HR")]
        public ActionResult EditExtensionNumber(bool isInternal, int empExtID)
        {
            var model = _reportService.GetEmployeeExtensionNoById(isInternal, empExtID);
            var group = _reportService.GetAllExtensionGroupInfo(isInternal, null);
            ViewBag.Group = group.Select(x => new SelectListItem
            {
                Text = x.TeamName,
                Value = x.TeamId.ToString()
            }).ToList();
            return View(model);
        }

        /// <summary>
        /// ActionResult EditExtensionNumber.
        /// </summary>
        /// <param name="model">ExtensionNoModel model.</param>
        /// <returns>View Index when edit successful.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult EditExtensionNumber(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            var group = _reportService.GetAllExtensionGroupInfo(model.IsInternal, null);
            ViewBag.Group = group.Select(x => new SelectListItem
            {
                Text = x.TeamName,
                Value = x.TeamId.ToString()
            }).ToList();
            if (ModelState.IsValid)
            {
                ViewBag.Updated = _reportService.UpdateExtensionNoMember(model);
            }

            return View();
        }

        public ActionResult ManageExternalGroup()
        {
            var group = _reportService.GetAllExtensionGroupInfo(false, null);
            return View(group);
        }

        /// <summary>
        /// Method Check External Employee.
        /// </summary>
        /// <param name="model">ExtensionNoModel model.</param>
        private void CheckExternalEmployee(ExtensionNoModel model)
        {
            if (model.IsInternal == false)
            {
                foreach (var modelError in ModelState)
                {
                    string propertyName = modelError.Key;

                    if (propertyName.Contains("EmpId"))
                    {
                        ModelState[propertyName].Errors.Clear();
                    }
                }
            }
        }
    }
}
