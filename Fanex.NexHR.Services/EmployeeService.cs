// --------------------------------------------------------------------------
// <copyright file="EmployeeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Employee Service.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private const HashProvider _HashProvider = HashProvider.SHA1;

        private readonly IShiftRepository _shiftRepository;
        private readonly ITeamService _teamService;
        private IEmployeeRepository _employeeRepository;

        public EmployeeService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._shiftRepository = new ShiftRepository();
            this._teamService = new TeamService();
        }

        public EmployeeService(IEmployeeRepository employeeRepository, IShiftRepository shifRepository)
        {
            this._employeeRepository = employeeRepository;
            this._shiftRepository = shifRepository;
            this._teamService = new TeamService();
        }

        public IList<Shift> GetAllShifts()
        {
            return this._shiftRepository.GetAll();
        }

        public IList<Employee> GetLeaders(int employeeId)
        {
            IList<Employee> leaders = new List<Employee>();
            if (employeeId > 0)
            {
                leaders = this._employeeRepository.GetLeaders(employeeId);
            }

            return leaders;
        }

        public Employee Get(int? employeeId = null, string domainUsername = null)
        {
            return this._employeeRepository.Get(employeeId, domainUsername);
        }

        public Employee Get(string code)
        {
            return this._employeeRepository.Get(code);
        }

        public IList<Employee> Query(string query, string groupIds, bool? enable = null, bool? isDelete = null, int? titleId = null)
        {
            return this._employeeRepository.Query(query, groupIds, enable, isDelete, titleId);
        }

        public IList<Employee> GetEmployeeForRelatedTeam(int employeeId)
        {
            return this._employeeRepository.GetEmployeeForRelatedTeam(employeeId);
        }

        public IList<Employee> GetMembersByManager(int managerId, short role, string keyword, int? groupId = null)
        {
            var members = this._employeeRepository.GetMembersByManager(managerId, role, keyword, groupId);

            return members;
        }

        public bool AdjustAnnualLeaves(
            string empIds,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy)
        {
            return this._employeeRepository.AdjustAnnualLeaves(
                empIds,
                description,
                fromDate,
                deductDays,
                creditDays,
                createdDate,
                createdBy);
        }

        public bool ResetAnnualLeaves(string empValue, string description, DateTime fromDate, int resetDays, DateTime createdDate, int createdBy) 
        {
            return this._employeeRepository.ResetAnnualLeaves(
                empValue, 
                description, 
                fromDate, 
                resetDays, 
                createdDate, 
                createdBy); 
        }

        public IList<Employee> GetAll()
        {
            return this._employeeRepository.GetAll();
        }

        public int Insert(ViewModels.CreateEmployeeViewModel model)
        {
            Check.Argument.IsNotNull(model, "model");

            // hash pasword
            model.Password = Encryptor.ComputeHash(_HashProvider, model.Password);
            return this._employeeRepository.Insert(model);
        }

        public bool Delete(int id, DateTime resignedDate)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");
            return this._employeeRepository.Delete(id, resignedDate);
        }

        public bool Update(Employee model)
        {
            Check.Argument.IsNotNull(model, "model");

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                model.Password = Encryptor.ComputeHash(_HashProvider, model.Password);
            }

            return this._employeeRepository.Update(model);
        }

        public EmployeeShift GetEmployeeShift(int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");
            return this._employeeRepository.GetEmployeeShift(id);
        }

        public bool UpdateEmployeeShift(EmployeeShift model)
        {
            Check.Argument.IsNotNull(model, "modmodelel");
            return this._employeeRepository.UpdateEmployeeShift(model);
        }

        public IList<WeekWorking> GetWeekWorking(int id, System.DateTime date)
        {
            Check.Argument.IsNotNull(id, "id");
            return this._employeeRepository.GetWeekWorking(id, date);
        }

        public bool UpdateWeekWorking(EmployeeShift model, System.DateTime date)
        {
            Check.Argument.IsNotNull(model, "model");
            return this._employeeRepository.UpdateWeekWorking(model, date);
        }

        public ViewModels.MySpaceViewModel GetMySpace(int id)
        {
            return this._employeeRepository.GetMySpace(id);
        }

        public HrLeaderDashboardModel GetHrLeaderDashboard(DateTime date, int? employeeId = null)
        {
            return this._employeeRepository.GetHrLeaderDashboard(date, employeeId);
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(System.DateTime date, int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");
            return this._employeeRepository.GetWorkingSchedule(date, id);
        }

        public IList<ViewModels.WorkingCalendarsViewModel> GetWorkingCalendars(string date)
        {
            var workingCalendarList = this._employeeRepository.GetWorkingCalendars(date);

            return workingCalendarList;
        }

        public bool UpdateShift(int id, System.DateTime date, int? shift, int modifiedBy, DateTime modifiedDate)
        {
            return this._employeeRepository.UpdateShift(id, date, shift, modifiedBy, modifiedDate);
        }

        public IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans)
        {
            return this._employeeRepository.GetAnnualLeaveTransactions(id, year, excludedRequestTrans);
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id)
        {
            return this._employeeRepository.GetAnnualLeaveTransaction(id);
        }

        public bool DeleteAnnualLeaveTransaction(long id, DateTime modifiedDate, int modifiedBy)
        {
            return this._employeeRepository.DeleteAnnualLeaveTransaction(id, modifiedDate, modifiedBy);
        }

        public bool UpdateAnnualLeaveTransaction(
            long id,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime modifiedDate,
            int modifiedBy)
        {
            return this._employeeRepository.UpdateAnnualLeaveTransaction(id, description, fromDate, deductDays, creditDays, modifiedDate, modifiedBy);
        }

        public bool InsertAnnualLeaveTransaction(
            int empId,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy)
        {
            return this._employeeRepository.InsertAnnualLeaveTransaction(empId, description, fromDate, deductDays, creditDays, createdDate, createdBy);
        }

        public IList<UserChangedShift> GetUserChangedShift(DateTime date)
        {
            return this._employeeRepository.GetUserChangedShift(date);
        }

        public IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null)
        {
            return this._employeeRepository.GetHalfDayOffs(reportDate, employeeId, groupId);
        }

        /// <summary>
        /// Function to update off day for all employee.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">ChangeWorkingCalendars ViewModel.</param>
        /// <returns>Result of updated.</returns>
        public bool UpdateOffDayForAllEmployees(ModelStateDictionary modelState, ChangeWorkingCalendarsViewModel model)
        {
            // Check.Argument.IsNotNull(model, "model");
            bool result = true;
            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                this._employeeRepository.UpdateOffDayForAllEmployees(model.Date, model.IsOfDay, model.ModifiedBy, model.ModifiedDate, out errorCode, out message);
                if (errorCode != 0)
                {
                    modelState.AddModelError(string.Empty, message);
                    result = false;
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
                result = false;
            }

            return result;
        }

        public int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return this._employeeRepository.GetRangeDate(employeeId, fromDate, toDate);
        }

        public IEnumerable<DateTime> GetDisableRequestDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return this._employeeRepository.GetDisableRequestDate(employeeId, fromDate, toDate);
        }

        public int ValidateUser(string code, string password)
        {
            Employee employee = this.Get(code);
            return this.ValidateUser(employee, password);
        }

        /// <summary>
        /// Function to change password of employee.
        /// </summary>
        /// <param name="empId">Employee Id.</param>
        /// <param name="oldPassword">Old password.</param>
        /// <param name="password">New password.</param>
        /// <returns>Result of changed password.</returns>
        public int ChangePassword(int empId, string oldPassword, string password)
        {
            Employee employee = this.Get(empId);
            int retCode = this.ValidateUser(employee, oldPassword);
            if (retCode == 0) 
            {
                string hashPassword = Encryptor.ComputeHash(_HashProvider, password);
                retCode = this._employeeRepository.ChangePassword(empId, hashPassword);
            }

            return retCode;
        }

        /// <summary>
        /// Function to validate user.
        /// </summary>
        /// <param name="employee">Employee need validate.</param>
        /// <param name="password">Password of employee.</param>
        /// <returns>Result of validate.</returns>
        private int ValidateUser(Employee employee, string password)
        {
            //// -1: wrong username
            ////  1: deleted
            ////  2: disabled
            ////  3: wrong password
            ////  0: success
            int retCode = -1;
            if (employee != null)
            {
                if (employee.IsDeleted) 
                {
                    //// deleted
                    retCode = 1;
                }
                else if (!employee.Enabled) 
                {
                    //// disabled
                    retCode = 2;
                }
                else if (string.IsNullOrWhiteSpace(employee.Password))
                {
                    //// wrong password
                    retCode = 3; 
                }
                else
                {
                    //// check password
                    if (Encryptor.CompareStringToHash(_HashProvider, password, employee.Password))
                    {
                        //// OK
                        retCode = 0; 
                    }
                    else
                    {
                        //// wrong password
                        retCode = 3; 
                    }
                }
            }

            return retCode;
        }
    }
}