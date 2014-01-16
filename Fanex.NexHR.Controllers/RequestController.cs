namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;    
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.Services;

    [Authorize]
    public class RequestController : BaseController
    {
        #region private services                
        private readonly IEmployeeService _employeeService;        
        private readonly IShiftService _shiftService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IRequestService _requestService;
        #endregion

        #region Ctor
        public RequestController()
            : this(null, null, null, null)
        { }

        public RequestController(
            
            IEmployeeService employeeService = null,
            IShiftService shiftService = null,
            ILeaveTypeService leaveTypeService = null,
            IRequestService requestService = null)
        {
            
            _employeeService = employeeService ?? new EmployeeService();
            _shiftService = shiftService ?? new ShiftService();
            _leaveTypeService = leaveTypeService ?? new LeaveTypeService();
            _requestService = requestService ?? new RequestService();
        }
        #endregion

        #region Check In/Out
        //public ActionResult CheckInOut()
        //{
        //    var employee = _employeeService.Get(_userService.CurrentUser.Id);
        //    ViewBag.Shifts = _shiftService.GetAll();
        //    return View(new CheckInOutRequestViewModel
        //    {
        //        Employee = employee.NickName,
        //        EmployeeId = employee.Id,
        //        ForDate = DateTime.Now.Date,
        //        Hour = DateTime.Now.Hour,
        //        Minute = DateTime.Now.Minute,
        //        Type = (short)RequestTypes.CheckIn,
        //        Shift = _shiftService.GetByDate(employee.Id, DateTime.Now).Name
        //    });
        //}

        //[HttpPost]
        //public ActionResult CheckInOut(CheckInOutRequestViewModel model)
        //{
        //    ViewBag.Shifts = _shiftService.GetAll();
        //    ViewBag.Requested = _requestService.CheckInOut(ModelState, model);
        //    return View(model);
        //}

        /// <summary>
        /// Add by shadow
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public ActionResult MyCheckInOutRequest(int year = 2012)
        {
            ViewBag.Year = year;
           // return View(_employeeService.GetCheckInOutRequest(143, year));

            return View();
        }

        public ActionResult UpdateCheckInOut(int id)
        {
            var request = _employeeService.GetWorkingTimeRequest(id);
            var employee = _employeeService.Get(request.EmpId);
            return View(new CheckInOutRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                ForDate = request.FromDate.Date,
                Hour = request.FromDate.Hour,
                Minute = request.FromDate.Minute,
                Type = request.RequestType
            });
        }

        [HttpPost]
        public ActionResult UpdateCheckInOut(CheckInOutRequestViewModel model)
        {
            ViewBag.Updated = _requestService.UpdateCheckInOut(ModelState, model);
            return View(model);
        }


        #endregion

        #region Late In/Early Out
        
        public ActionResult LateInEarlyOut()
        {
            var employee = _employeeService.Get(this.CurrentUser.Id);
            return View(new LateInEarlyOutRequestViewModel
            {
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                ForDate = DateTime.Now.Date,
                Hour = 0,
                Minute = 0
            });
        }

        [HttpPost]
        public ActionResult LateInEarlyOut(LateInEarlyOutRequestViewModel model)
        {
            ViewBag.Requested = _requestService.LateInEarlyOut(ModelState, model);
            return View(model);
        }

        public ActionResult UpdateLateInEarlyOut(int id)
        {
            var request = _employeeService.GetWorkingTimeRequest(id);
            var employee = _employeeService.Get(request.EmpId);
            int minute = request.RequestType == (short)RequestTypes.LateIn
                ? (int)request.LateInMins
                : (int)request.EarlyOutMins;

            return View(new LateInEarlyOutRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                ForDate = request.FromDate.Date,
                Hour = minute / 60,
                Minute = minute % 60,
                Type = request.RequestType
            });
        }

        [HttpPost]
        public ActionResult UpdateLateInEarlyOut(LateInEarlyOutRequestViewModel model)
        {
            ViewBag.Updated = _requestService.UpdateLateInEarlyOut(ModelState, model);
            return View(model);
        }
        #endregion

        #region Leave
        public ActionResult Leave()
        {
            var employee = _employeeService.Get(this.CurrentUser.Id);
            ViewBag.LeaveTypes = _leaveTypeService.GetAll();
            var model = new LeaveRequestViewModel
            {
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Leave(LeaveRequestViewModel model)
        {
            ViewBag.LeaveTypes = _leaveTypeService.GetAll();
            ViewBag.Requested = _requestService.Leave(ModelState, model);
            return View(model);
        }

        public ActionResult UpdateLeave(int id)
        {
            var request = _employeeService.GetWorkingTimeRequest(id);
            var employee = _employeeService.Get(request.EmpId);
            var model = new LeaveRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Value.Date,
                TotalDays = (decimal)request.TotalDays,
                LeaveTypeId = (int)request.LeaveTypeID,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark
            };

            ViewBag.LeaveTypes = _leaveTypeService.GetAll();
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateLeave(LeaveRequestViewModel model)
        {
            ViewBag.LeaveTypes = _leaveTypeService.GetAll();
            ViewBag.Updated = _requestService.UpdateLeave(ModelState, model);
            return View(model);
        }

        #endregion   

        #region Change Shift       

        public ActionResult ChangeShift()
        {
            var employee = _employeeService.Get(this.CurrentUser.Id);
            var shift = _shiftService.GetByDate(employee.Id, DateTime.Now.Date);
            var model = new ChangeShiftViewModel
            {
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                ForDate = DateTime.Now.Date,
                FromShift = (int)shift.Id,
                FromShiftName = shift.Name
            };
            ViewBag.Shifts = _shiftService.GetAll();
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeShift(ChangeShiftViewModel model)
        {
            ViewBag.Shifts = _shiftService.GetAll();
            ViewBag.Requested = _requestService.ChangeShift(ModelState, model);
            return View(model);
        }

        public ActionResult UpdateChangeShift(int id)
        {
            var request = _employeeService.GetWorkingTimeRequest(id);
            var shift = _shiftService.GetByDate(this.CurrentUser.Id, request.FromDate.Date);
            var employee = _employeeService.Get(request.EmpId);
            var model = new ChangeShiftViewModel
            {
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                Id = request.Id,
                ForDate = request.FromDate.Date,
                FromShift = (int)shift.Id,
                FromShiftName = shift.Name,
                ToShift = (int)request.ToShift,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark
            };

            ViewBag.Shifts = _shiftService.GetAll();
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateChangeShift(ChangeShiftViewModel model)
        {
            ViewBag.Shifts = _shiftService.GetAll();
            ViewBag.Updated = _requestService.UpdateChangeShift(ModelState, model);
            return View(model);
        }
        #endregion
    }
}
