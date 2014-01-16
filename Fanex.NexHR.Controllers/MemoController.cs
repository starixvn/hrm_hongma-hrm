// --------------------------------------------------------------------------
// <copyright file="MemoController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Memo Controller.
    /// </summary>
    [Authorize]
    public class MemoController : Controller
    {
        private readonly IMemoService _memoService;
        private readonly ILookupDataService _lookupDataService;

        public MemoController(IMemoService memoService = null, ILookupDataService lookupDataService = null)
        {
            _memoService = memoService ?? new MemoService();
            _lookupDataService = lookupDataService ?? new LookupDataService();            
        }

        public MemoController()
            : this(null)
        { 
        }

        /// <summary>
        /// ActionResult Index.
        /// </summary>
        /// <returns>View Index.</returns>
        public ActionResult Index()
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            MemoViewModel model = new MemoViewModel
            {
                Content = string.Empty,
                Year = DateTime.Now.Year,
                Category = -1
            };

            return Index(model);
        }

        [HttpPost]
        public ActionResult Index(MemoViewModel model)
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            ViewBag.EntityList = _memoService.GetAll(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            return View(_memoService.GetById(id));
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="model">Memo model.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Memo model)
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            if (model.Type == 1 && string.IsNullOrEmpty(model.AttachFile))
            {
                return View(model);
            }

            if (model.Type == 1)
            {
                model.Contents = model.AttachFile;
            }

            if (ModelState.IsValid)
            {
                ViewBag.Updated = _memoService.Update(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            return View(new Memo
            {
                SentTo = "All Staff",
                SentFrom = "Management",
                TotalPages = 1,
                SentDate = DateTime.Now,
                Contents = "[Contents]",
                Type = 1
            });
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">Memo model.</param>
        /// <returns>View Create.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Memo model)
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            if (model.Type == 1)
            {
                model.Contents = model.AttachFile;
            }

            if (ModelState.IsValid)
            {
                var id = _memoService.Insert(model);

                ViewBag.Updated = id != -1 ? true : false;
                if (id == -1)
                {
                    ModelState.AddModelError(string.Empty, "Duplicate memo");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        public ActionResult Detail(int id)
        {
            return View(_memoService.GetById(id));
        }
    }
}