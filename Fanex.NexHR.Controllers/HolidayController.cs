// --------------------------------------------------------------------------
// <copyright file="HolidayController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Holiday Controller.
    /// </summary>
    [Authorize]
    public class HolidayController : BaseController
    {
        private readonly IHolidayService _holidayService;

        public HolidayController(IHolidayService holidayService = null)
        {
            _holidayService = holidayService ?? new HolidayService();
        }

        public HolidayController()
            : this(null)
        { 
        }

        public ActionResult Index()
        {
            return View(_holidayService.GetByYear(DateTime.Now.Year));
        }

        [HttpGet]
        public ActionResult Index(int? year)
        {
            ViewBag.year = year ?? DateTime.Now.Year;
            return View(_holidayService.GetByYear(ViewBag.year));
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            return View(_holidayService.GetById(id));
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="model">Holiday model.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Edit(Holiday model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;
                model.ModifiedBy = this.CurrentUser.Id;
                ViewBag.Updated = _holidayService.Update(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            return View(new Holiday
            {
                Name = string.Empty,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedBy = this.CurrentUser.Id
            });
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">Holiday model.</param>
        /// <returns>View Create.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(Holiday model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;
                model.ModifiedBy = this.CurrentUser.Id;
                var id = _holidayService.Insert(model);
                if (id == -1)
                {
                    ViewBag.Updated = false;
                    ModelState.AddModelError(string.Empty, "Duplicate holiday");
                }
                else
                {
                    ViewBag.Updated = true;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }
    }
}