// --------------------------------------------------------------------------
// <copyright file="RequestController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Request Controller.
    /// </summary>
    [Authorize]
    public class RequestController : BaseController
    {
        #region private services
        protected readonly IEmployeeService _EmployeeService;
        protected readonly IShiftService _ShiftService;
        protected readonly ILeaveTypeService _LeaveTypeService;
        protected readonly IRequestService _RequestService;
        protected readonly ITeamService _TeamService;
        #endregion

        #region Ctor
        public RequestController()
            : this(null, null, null, null)
        {
        }

        public RequestController(
            IEmployeeService employeeService = null,
            IShiftService shiftService = null,
            ILeaveTypeService leaveTypeService = null,
            IRequestService requestService = null,
            ITeamService teamService = null)
        {
            _EmployeeService = employeeService ?? new EmployeeService();
            _ShiftService = shiftService ?? new ShiftService();
            _LeaveTypeService = leaveTypeService ?? new LeaveTypeService();
            _RequestService = requestService ?? new RequestService();
            _TeamService = teamService ?? new TeamService();
        }
        #endregion

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        /// <summary>
        /// ActionResult Member.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <returns>View Member.</returns>
        [Authorize(Roles = "AssistantLeader")]
        public ActionResult Member(RequestViewModel request)
        {
            if (request.StatusId == -1)
            {
                request.StatusId = (byte)RequestStatuses.Pending;
            }
            
            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Leader = true;
            ViewBag.Action = "Member";

            CommonViewRequest(ref request, RolesEnum.AssistantLeader, false);

            return View(request);
        }

        /// <summary>
        ///  ActionResult HumanResourceView.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <returns>View HumanResource.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HRView(RequestViewModel request)
        {
            if (request.StatusId == -1)
            {
                request.StatusId = 0;
            }

            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Action = "HRView";

            CommonViewRequest(ref request, RolesEnum.HR, false);

            return View(request);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <returns>View Employee.</returns>
        public ActionResult EmployeeView(RequestViewModel request)
        {
            if (request.StatusId == -1)
            {
                request.StatusId = 0;
            }

            ViewBag.IsAllowChangeStatus = false;
            ViewBag.Action = "EmployeeView";

            CommonViewRequest(ref request, RolesEnum.HR, false);

            return View("HRView", request);
        }

        /// <summary>
        /// ActionResult DailyView.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <returns>View DailyRequest.</returns>
        public ActionResult DailyView(RequestViewModel request)
        {
            if (request.StatusId == -1)
            {
                request.StatusId = 0;
            }

            ViewBag.IsAllowChangeStatus = false;
            ViewBag.Action = "DailyView";

            CommonViewRequest(ref request, RolesEnum.HR, true);

            return View("DailyRequestsView", request);
        }

        /// <summary>
        /// ActionResult HumanResourceViewHistory.
        /// </summary>
        /// <param name="id">CheckInOutRequestHistory id.</param>
        /// <param name="type">CheckInOutRequestHistory type.</param>
        /// <returns>View HumanResourceViewHistory.</returns>
        public ActionResult HRViewHistory(long id, int type)
        {
            switch (type)
            {
                case 1:
                    GetCheckInOutRequestHistory(id, type);
                    break;
                case 2:
                    GetLateInEarlyOutRequestHistory(id, type);
                    break;
                case 3:
                    GetChangeShiftRequestHistory(id, type);
                    break;
                case 4:
                    GetLeaveRequestHistory(id, type);
                    break;
                case 5:
                    GetOvertimeRequestHistory(id, type);
                    break;
            }

            ViewBag.requestId = id;
            ViewBag.requestType = type;

            return View();
        }

        [HttpPost]
        public bool DeleteWorkingTimeRequest(int id)
        {
            return _RequestService.DeleteWorkingTimeRequest(id);
        }

        /// <summary>
        /// Check if user can and has permission to edit a request.
        /// </summary>
        /// <param name="status">Status of request.</param>
        /// <param name="employeeId">Which employee request belong to.</param>
        /// <param name="creatorId">The person who creates this request.</param>
        /// <returns>Return true if current user can edit this request.</returns>
        protected bool Editable(byte status, int employeeId, int creatorId)
        {
            if (status != (byte)RequestStatuses.Pending ||
                (employeeId != this.CurrentUser.Id && creatorId != this.CurrentUser.Id))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Function CommonViewRequest.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <param name="role">List number of Roles.</param>
        /// <param name="isDailyRequest">Is Daily Request.</param>
        [NonAction]
        private void CommonViewRequest(ref RequestViewModel request, RolesEnum role, bool isDailyRequest)
        {
            if (role == RolesEnum.AssistantLeader)
            {
                if (request.TeamIds == null)
                {
                    request.ToDate = DateTime.Now.Date;
                    request.FromDate = request.ToDate.AddDays(-7);
                }

                request.TeamCollection = _TeamService.GetAll();
            }
            else
            {
                request.TeamCollection = _TeamService.GetAll();
            }

            var requestList = _RequestService.GetWorkingTimeRequests(
                request,
                (short)role,
                isDailyRequest,
                role == RolesEnum.AssistantLeader ? new int?(this.CurrentUser.Id) : null);

            ViewBag.Options = request;
            ViewBag.IsDailyRequest = isDailyRequest;
            ViewBag.EntityList = requestList;
        }

        [NonAction]
        private void GetLeaveRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _RequestService.GetLeaveRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        [NonAction]
        private void GetChangeShiftRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _RequestService.GetChangeShiftRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        [NonAction]
        private void GetLateInEarlyOutRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _RequestService.GetLateInEarlyOutRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        [NonAction]
        private void GetCheckInOutRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _RequestService.GetCheckInOutRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        [NonAction]
        private void GetOvertimeRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _RequestService.GetOvertimeRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }
    }
}
