// --------------------------------------------------------------------------
// <copyright file="EmployeeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.7.3">Created</change>
//   <change who="Anders.Le" date="2012.7.3">Modify avatar function</change>
// </history>
namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;

    /// <summary>
    /// Employee Controller.
    /// </summary>
    [Authorize]
    public class EmployeeController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;

        public EmployeeController()
            : this(null, null, null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService = null,
            ITeamService teamService = null,
            ILookupDataService lookupDataService = null,
            IShiftService shiftService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _shiftService = shiftService ?? new ShiftService();
        }

        public ActionResult Index()
        {
            ViewBag.Groups = _teamService.GetAll();
            return View();
        }

        /// <summary>
        /// ActionResult UserManagement.
        /// </summary>
        /// <param name="jobTitleId">Job Title Id.</param>
        /// <returns>View User Management.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult UserManagement(string jobTitleId = "")
        {
            ViewBag.JobTitleId = jobTitleId;
            ViewBag.Groups = _teamService.GetAll();

            var listJobTitle = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == jobTitleId
            }).ToList();

            return View();
        }

        /// <summary>
        /// ActionResult Detail.
        /// </summary>
        /// <param name="id">Employee's id.</param>
        /// <returns>View Details.</returns>
        public ActionResult Detail(int id)
        {
            var model = _employeeService.Get(id);

            ViewBag.JobTitle = (model.JobTitleId == null)
            ? Resources.Labels.Unknown
            : _lookupDataService.Get(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;

            ViewBag.Working = _employeeService.GetMySpace(id);
            var workingCalendar = _employeeService.GetEmployeeShift(id);
            var shifts = _shiftService.GetAll();

            ViewBag.Sun = workingCalendar.SunShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.SunShift).Name;

            ViewBag.Mon = workingCalendar.MonShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.MonShift).Name;

            ViewBag.Tue = workingCalendar.TueShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.TueShift).Name;

            ViewBag.Wed = workingCalendar.WedShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.WedShift).Name;

            ViewBag.Thu = workingCalendar.ThuShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.ThuShift).Name;

            ViewBag.Fri = workingCalendar.FriShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.FriShift).Name;

            ViewBag.Sat = workingCalendar.SatShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.SatShift).Name;

            return View(model);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            ViewBag.Shifts = _shiftService.GetSelectList();

            var teamList = _teamService.GetAll();
            ViewBag.TeamList = teamList;

            return View(new CreateEmployeeViewModel
            {
                MonShift = 1,
                TueShift = 1,
                WedShift = 1,
                ThuShift = 1,
                FriShift = 1,
            });
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">CreateEmployee ViewModel.</param>
        /// <returns>View Create.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            model.ProcessBy = this.CurrentUser.Id;
            ViewBag.Shifts = _shiftService.GetSelectList();

            var teamList = _teamService.GetAll();
            ViewBag.TeamList = teamList;

            if (ModelState.IsValid)
            {
                model.DomainUsername = ConfigurationManager.AppSettings["domain"] + model.DomainUsername;
                var id = _employeeService.Insert(model);
                if (id == -1)
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.DuplicateEmployeeCode);
                }
                else
                {
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="id">Employee's id.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            var model = _employeeService.Get(id);

            // sub teams
            var subTeams = _teamService.GetSubTeamsOfEmployee(id);
            var subGroups = string.Empty;
            if (subTeams != null && subTeams.Count > 0)
            {
                foreach (var team in subTeams)
                {
                    subGroups += team.Id + ",";
                }

                subGroups = subGroups.Substring(0, subGroups.Length - 1);
            }

            model.SubGroups = subGroups;

            // teams handle requests
            var teamsHandleRequests = _teamService.GetTeamsHandleRequestsOfEmployee(id);
            var leaderGroups = string.Empty;
            if (teamsHandleRequests != null && teamsHandleRequests.Count > 0)
            {
                foreach (var team in teamsHandleRequests)
                {
                    leaderGroups += team.Id + ",";
                }

                leaderGroups = leaderGroups.Substring(0, leaderGroups.Length - 1);
            }

            model.LeaderGroups = leaderGroups;

            ViewBag.Groups = _teamService.GetAll();
            ViewBag.JobTitles = _lookupDataService.GetAll(LookupCodeName.JobTitle);

            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="model">Employee model.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee model)
        {
            model.Modifier = this.CurrentUser;

            if (ModelState.IsValid)
            {
                if (model.SubGroupArray != null && model.SubGroupArray.Contains(model.Team.Id.ToString()))
                {
                    model.SubGroupArray = model.SubGroupArray.Where(i => i != model.Team.Id.ToString()).ToArray();
                }

                var result = _employeeService.Update(model);
                ViewBag.Updated = result;

                // update current user?
                if (result && this.CurrentUser.Id == model.Id)
                {
                    var currentUser = _employeeService.Get(model.Id);
                    HttpContext.Session["CurrentUser"] = currentUser;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            ViewBag.Groups = _teamService.GetAll();
            ViewBag.JobTitles = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult DefaultSchedule(int id)
        {
            ViewBag.Employee = _employeeService.Get(id);
            ViewBag.Shifts = _shiftService.GetSelectList();

            return View(_employeeService.GetEmployeeShift(id));
        }

        /// <summary>
        /// ActionResult DefaultSchedule.
        /// </summary>
        /// <param name="model">EmployeeShift model.</param>
        /// <returns>View DefaultSchedule.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult DefaultSchedule(EmployeeShift model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Updated = _employeeService.UpdateEmployeeShift(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            ViewBag.Employee = _employeeService.Get(model.EmpID);

            ViewBag.Shifts = _shiftService.GetSelectList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult MonthSchedule(int id)
        {
            ViewBag.Id = id;
            ViewBag.Employee = _employeeService.Get(id);

            ViewBag.Shifts = _shiftService.GetSelectList();

            return View();
        }

        /// <summary>
        /// FileResult Avatar.
        /// </summary>
        /// <param name="id">Image name and format.</param>
        /// <returns>File name.</returns>
        [AllowAnonymous]
        public FileResult Avatar(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = this.CurrentUser.Code + ".jpg";
            }
            else
            {
                id += ".jpg";
            }

            if (System.IO.File.Exists(Server.MapPath("/avatars/" + id)))
            {
                return File("/avatars/" + id, "image/jpeg");
            }

            return File("/avatars/avatar.jpg", "image/jpeg");
        }

        /// <summary>
        /// Action UploadAvatar.
        /// </summary>
        /// <param name="code">File name.</param>
        /// <param name="file">Http Posted File Base.</param>
        /// <returns>Type of format.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult UploadAvatar(string code, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var stream = new MemoryStream();
                var processor = new ImageProcessor();
                var resizer = new ResizeFilter(64, 64, ResizeMethod.Crop, AnchorLocation.Middle);
                processor.AddFilter(resizer);
                processor.OutputFormat = OutputFormat.Jpeg;
                processor.JpegCompression = 100;
                processor.ProcessImage(file.InputStream, stream);

                var image = Image.FromStream(stream);
                image.Save(Server.MapPath("/avatars/") + code + ".jpg");

                return Json(true);
            }

            return Json(null);
        }

        public ActionResult MySpace()
        {
            return View(_employeeService.GetMySpace(this.CurrentUser.Id));
        }

        public ActionResult MyWorkingSchedule()
        {
            return View(_employeeService.GetWorkingSchedule(DateTime.Now, this.CurrentUser.Id));
        }

        /// <summary>
        /// ActionResult WorkingCalendars.
        /// </summary>
        /// <param name="month">Current month.</param>
        /// <param name="year">Current year.</param>
        /// <returns>View WorkingCalendars.</returns>
        public ActionResult WorkingCalendars(int month = 0, int year = 0)
        {
            var currentDate = new System.Text.StringBuilder();

            ViewBag.Year = year != 0 ? year : DateTime.Now.Year;
            ViewBag.Month = month != 0 ? month : DateTime.Now.Month;

            currentDate.AppendFormat("{0}/{1}/1", ViewBag.Year, ViewBag.Month);
            ViewBag.CurrentDate = currentDate;

            var userChangedShift = _employeeService.GetUserChangedShift(new DateTime(ViewBag.Year, ViewBag.Month, 1));

            ViewBag.UserChangeShift = userChangedShift;

            ViewBag.HalfDayOffs = _employeeService.GetHalfDayOffs(DateTime.Parse(currentDate.ToString()));

            return View(_employeeService.GetWorkingCalendars(currentDate.ToString()));
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public ActionResult WorkingCalendarsEdit()
        {
            var model = new ChangeWorkingCalendarsViewModel
            {
                Date = DateTime.Now,
                IsOfDay = 0,
                ModifiedDate = DateTime.Now,
                ModifiedBy = this.CurrentUser.Id
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult WorkingCalendarsEdit(ChangeWorkingCalendarsViewModel model)
        {
            model.ModifiedDate = DateTime.Now;
            model.ModifiedBy = this.CurrentUser.Id;
            ViewBag.Requested = _employeeService.UpdateOffDayForAllEmployees(ModelState, model);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ManagementDashboard()
        {
            if (this.CurrentUser.IsInRoles(RolesEnum.AssistantLeader))
            {
                return PartialView(_employeeService.GetHrLeaderDashboard(DateTime.Today, this.CurrentUser.Id));
            }

            return Content(string.Empty);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TeamManagement()
        {
            IList<Team> teams = _teamService.GetAll();

            return View(teams);
        }

        /// <summary>
        /// ActionResult AddTeam.
        /// </summary>
        /// <param name="teamId">Id of new team.</param>
        /// <returns>View AddTeam.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult AddTeam(int teamId = -1)
        {
            TeamViewModel model = null;

            // create new
            if (teamId == -1)
            {
                model = new TeamViewModel
                {
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };
            }
            else
            {
                var team = _teamService.Get(teamId);
                if (team != null)
                {
                    model = new TeamViewModel
                    {
                        Id = teamId,
                        TeamCode = team.Code,
                        TeamName = team.Name,
                        AssistantLeaderId = team.AssistantLeader.Id,
                        AssistantLeader = team.AssistantLeader.DisplayName,
                        LeaderId = team.Leader.Id,
                        Leader = team.Leader.DisplayName
                    };
                }
                else
                {
                    // throw exception
                }
            }

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult ChangePwd()
        {
            return View();
        }

        /// <summary>
        /// ActionResult Change Password.
        /// </summary>
        /// <param name="model">ChangePassword ViewModel.</param>
        /// <returns>View Change Password.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                int retCode = _employeeService.ChangePassword(this.CurrentUser.Id, model.Password, model.NewPassword);
                switch (retCode)
                {
                    case 0: // OK
                        ViewBag.ChangePwdOK = Resources.Messages.ChangePwdOK;
                        break;

                    case 1: // deleted
                        ModelState.AddModelError(string.Empty, Resources.Messages.DeletedUser);
                        break;

                    case 2: // disabled
                        ModelState.AddModelError(string.Empty, Resources.Messages.DisabledUser);
                        break;

                    case -1:
                    case 3:
                    default: // wrong password
                        ModelState.AddModelError(string.Empty, Resources.Messages.WrongPassword);
                        break;
                }
            }

            return View(model);
        }
    }
}
