// --------------------------------------------------------------------------
// <copyright file="AnnualLeaveBalanceController.cs" company="Nexcel Solutions Vietnam">
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
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Controllers.Context;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// AnnualLeaveBalance Controller.
    /// </summary>
    [Authorize(Roles = "HR")]
    public class AnnualLeaveBalanceController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IReportService _reportService;

        public AnnualLeaveBalanceController()
        {
            _employeeService = new EmployeeService();
            _teamService = new TeamService();
            _reportService = new ReportService();
        }

        public AnnualLeaveBalanceController(IEmployeeService employeeService, ITeamService teamService, IReportService reportService)
        {
            _employeeService = employeeService;
            _teamService = teamService;
            _reportService = reportService ?? new ReportService();
        }

        /// <summary>
        /// ActionResult Index.
        /// </summary>
        /// <param name="teamIds">Team's id.</param>
        /// <param name="employee">Employee name.</param>
        /// <param name="year">Current Year.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="minAnnualBalance">Min Annual Balance.</param>
        /// <returns>Index View.</returns>
        public ActionResult Index(
            string teamIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            int? minAnnualBalance = null)
        {
            ViewBag.IsShowCloseBalance = AppSetting.IsShowCloseBalance;

            var model = new LeaveSummaryViewModel
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year,
                TeamIds = teamIds,
                Employee = employee,
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                MinAnnualBalance = minAnnualBalance.HasValue ? minAnnualBalance.Value : 0
            };

            return Index(model);
        }

        /// <summary>
        /// ActionResult Index.
        /// </summary>
        /// <param name="model">LeaveSummary ViewModel.</param>
        /// <returns>View Index.</returns>
        [HttpPost]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            ViewBag.IsShowCloseBalance = AppSetting.IsShowCloseBalance;

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
            ViewBag.MinAnnualBalance = model.MinAnnualBalance;

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = _reportService.GetAnnualLeaveByTeam(entityList, teams);

            foreach (LeaveSummaryByTeam item in leaveSummaryByTeamList)
            {
                item.LeaveSummaryInTeam = item.LeaveSummaryInTeam.Where(m => m.LeaveBalance >= model.MinAnnualBalance).ToList();
            }

            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult MultipleInsert()
        {
            return PartialView(new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        [ChildActionOnly]
        public PartialViewResult MultipleReset()
        {
            return PartialView(new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        /// <summary>
        /// ActionResult Employee.
        /// </summary>
        /// <param name="id">Employee's Id.</param>
        /// <returns>View AnnualLeaveBalance of Employee. </returns>
        public ActionResult Employee(int id)
        {
            ViewBag.Employee = _employeeService.Get(id);

            var model = new AnnualLeaveBalanceViewModel
               {
                   EmployeeId = id,
                   Year = DateTime.Now.Year,
                   ExcludedRequestTrans = true
               };

            ViewBag.Data = _employeeService.GetAnnualLeaveTransactions(
                model.EmployeeId,
                model.Year,
                false);

            return View(model);
        }

        /// <summary>
        /// ActionResult Employee.
        /// </summary>
        /// <param name="model">AnnualLeaveBalance ViewModel.</param>
        /// <returns>View AnnualLeaveBalance of Employee.</returns>
        [HttpPost]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            model.ExcludedRequestTrans = model.ExcludedRequestTrans ? false : true;

            ViewBag.Employee = _employeeService.Get(model.EmployeeId);
            ViewBag.Data = _employeeService.GetAnnualLeaveTransactions(
                model.EmployeeId,
                model.Year,
                model.ExcludedRequestTrans);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _employeeService.GetAnnualLeaveTransaction(id);
            ViewBag.Employee = _employeeService.Get(model.EmpId);
            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="formCollection">Form Collection.</param>
        /// <param name="model">AnnualLeaveBalance Model.</param>
        /// <returns>View Edit.</returns>
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, AnnualLeaveBalanceModel model)
        {
            ViewBag.Employee = _employeeService.Get(model.EmpId);
            var command = formCollection["command"];

            if (command == Resources.Labels.Delete)
            {
                var result = _employeeService.DeleteAnnualLeaveTransaction(model.Id, DateTime.Now, this.CurrentUser.Id);
                if (result)
                {
                    return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.ErrorTryAgain);
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                // update annual leave here
                ViewBag.Updated = _employeeService.UpdateAnnualLeaveTransaction(
                    model.Id, 
                    model.Description,
                    model.AffectedFrom, 
                    model.DeductDays, 
                    model.CreditDays, 
                    DateTime.Now, 
                    this.CurrentUser.Id);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        public ActionResult Create(int id)
        {
            ViewBag.Employee = _employeeService.Get(id);
            return View(new AnnualLeaveBalanceModel
            {
                EmpId = id,
                AffectedFrom = DateTime.Now
            });
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">AnnualLeaveBalance Model.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(AnnualLeaveBalanceModel model)
        {
            ViewBag.Employee = _employeeService.Get(model.EmpId);

            if (ModelState.IsValid)
            {
                ViewBag.Inserted = _employeeService.InsertAnnualLeaveTransaction(
                    model.EmpId,
                    model.Description, 
                    model.AffectedFrom, 
                    model.DeductDays, 
                    model.CreditDays, 
                    DateTime.Now, 
                    this.CurrentUser.Id);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }
    }
}