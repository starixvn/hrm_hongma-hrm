// --------------------------------------------------------------------------
// <copyright file="EmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.23">Created</change>
//   <change who="Anders.Le" date="2012.5.23">Modify Get function</change>
// </history>
namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Dapper;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Employee Repository.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        /// <summary>
        /// Method get Employee.
        /// </summary>
        /// <param name="employeeId">The id of an employee.</param>
        /// <param name="domainUsername">Domain username.</param>
        /// <returns>Employee entity.</returns>
        public Employee Get(int? employeeId, string domainUsername = null)
        {
            using (var db = new ObjectDb("GetEmployee"))
            {
                var entity = db.Query<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameterValues: new { Id = employeeId, domainUserName = domainUsername }).FirstOrDefault();

                return entity;
            }
        }

        /// <summary>
        /// Method get Employee by code.
        /// </summary>
        /// <param name="code">Employee's code.</param>
        /// <returns>Employee entity.</returns>
        public Employee Get(string code)
        {
            using (var db = new ObjectDb("Employee_GetEmployeeByCode"))
            {
                Employee entity = db.Query<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameterValues: new { code = code }).FirstOrDefault();

                return entity;
            }
        }

        /// <summary>
        /// Method get all Employees.
        /// </summary>
        /// <returns>All employee.</returns>
        public IList<Employee> GetAll()
        {
            using (var db = new ObjectDb("GetEmployees"))
            {
                var p = new DynamicParameters();
                p.Add("@query", null);
                p.Add("@pageNumber", 1);
                p.Add("@pageSize", int.MaxValue);
                p.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var entityList = db.Query<Employee, dynamic, Employee>(
                       (e, t) =>
                       {
                           e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                           return e;
                       },
                       splitOn: "TeamID",
                       parameterValues: p);

                return entityList.ToList();
            }
        }

        /// <summary>
        /// Query Employee.
        /// </summary>
        /// <param name="query">String query.</param>
        /// <param name="groupIds">The group's id.</param>
        /// <param name="enable">Employee's status.</param>
        /// <param name="isDelete">Check available.</param>
        /// <param name="titleId">Title's id.</param>
        /// <returns>A List of Employee entity.</returns>
        public IList<Employee> Query(
            string query, 
            string groupIds,
            bool? enable = null,
            bool? isDelete = null, 
            int? titleId = null)
        {
            using (var db = new ObjectDb("GetEmployees"))
            {
                var p = new DynamicParameters();
                int? disableUsers, deleteUsers;
                if (enable == true)
                {
                    disableUsers = 0;
                }
                else if (enable == false)
                {
                    disableUsers = 1;
                }
                else
                {
                    disableUsers = null;
                }

                if (isDelete == true)
                {
                    deleteUsers = 1;
                }
                else if (isDelete == false)
                {
                    deleteUsers = 0;
                }
                else
                {
                    deleteUsers = null;
                }

                p.Add("@query", query);
                p.Add("@groupIds", groupIds);
                p.Add("@enable", disableUsers);
                p.Add("@isDelete", deleteUsers);
                p.Add("@pageNumber", 1);
                p.Add("@pageSize", int.MaxValue);
                p.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@titleId", titleId);
                var entityList = db.Query<Employee, dynamic, Employee>(
                     (e, t) =>
                     {
                         e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                         return e;
                     },
                     splitOn: "TeamID",
                     parameterValues: p);
                return entityList.ToList();
            }
        }

        public IList<Employee> GetEmployeeForRelatedTeam(int employeeId)
        {
            using (var db = new ObjectDb("Employee_GetEmployeeForRelatedTeam"))
            {
                var entityList = db.Query<Employee>(new { empId = employeeId }).ToList();

                return entityList;
            }
        }

        public IList<Employee> GetLeaders(int employeeId)
        {
            using (var db = new ObjectDb("Employee_GetLeaders"))
            {
                return db.Query<Employee>(new { id = employeeId }).ToList();
            }
        }

        /// <summary>
        /// Method insert an Employee.
        /// </summary>
        /// <param name="model">Employee View Model.</param>
        /// <returns>Id of inserted Employee.</returns>
        public int Insert(CreateEmployeeViewModel model)
        {
            using (var db = new ObjectDb("Employee_Insert"))
            {
                db.AddInParameter("@fullName", DbType.String, model.FullName);
                db.AddInParameter("@nickName", DbType.String, model.NickName);
                db.AddInParameter("@domainUsername", DbType.String, model.DomainUsername);
                db.AddInParameter("@code", DbType.String, model.Code);
                db.AddInParameter("@monShift", DbType.String, model.MonShift);
                db.AddInParameter("@tueShift", DbType.String, model.TueShift);
                db.AddInParameter("@wedShift", DbType.String, model.WedShift);
                db.AddInParameter("@thuShift", DbType.String, model.ThuShift);
                db.AddInParameter("@friShift", DbType.String, model.FriShift);
                db.AddInParameter("@satShift", DbType.String, model.SatShift);
                db.AddInParameter("@sunShift", DbType.String, model.SunShift);
                db.AddInParameter("@mainGroupId", DbType.Int16, model.TeamId);
                db.AddInParameter("@createddate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@createBy", DbType.Int16, model.ProcessBy);
                db.AddInParameter("@password", DbType.String, model.Password);

                return (int)db.ExecuteScalar();
            }
        }

        public bool Delete(int id, DateTime resignedDate)
        {
            using (var db = new ObjectDb("Employee_Delete"))
            {
                db.AddInParameter("@id", DbType.Int32, id);
                db.AddInParameter("@resignedDate", DbType.DateTime, resignedDate);
                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Method update Employee.
        /// </summary>
        /// <param name="model">Employee model.</param>
        /// <returns>Boolean to check success or not.</returns>
        public bool Update(Employee model)
        {
            using (var db = new ObjectDb("Employee_UpdateEmployee"))
            {
                db.AddInParameter("@id", DbType.Int32, model.Id);
                db.AddInParameter("@code", DbType.String, model.Code.Trim());
                db.AddInParameter("@EmployeeCard", DbType.String, model.EmployeeCard.Trim());
                db.AddInParameter("@domainUsername", DbType.String, model.DomainUsername);
                db.AddInParameter("@fullName", DbType.String, model.FullName);
                db.AddInParameter("@nickName", DbType.String, model.NickName);
                db.AddInParameter("@dob", DbType.DateTime, model.DOB);
                db.AddInParameter("@email", DbType.String, model.Email);
                db.AddInParameter("@jobTitleId", DbType.Int32, model.JobTitleId);
                db.AddInParameter("@enabled", DbType.Boolean, model.Enabled);
                db.AddInParameter("@phone", DbType.String, model.Phone);
                db.AddInParameter("@gender", DbType.Int32, model.Gender);
                db.AddInParameter("@mainGroupId", DbType.Int32, model.Team.Id);
                db.AddInParameter("@subGroups", DbType.String, model.SubGroups);
                db.AddInParameter("@HandleRequestGroupIDs", DbType.String, model.LeaderGroups);
                db.AddInParameter("@roles", DbType.String, model.Roles);
                db.AddInParameter("@joinedDate", DbType.DateTime, model.JoinedDate);
                db.AddInParameter("@confirmedDate", DbType.DateTime, model.ConfirmedDate);
                db.AddInParameter("@resignedDate", DbType.DateTime, model.ResignedDate);
                db.AddInParameter("@skype", DbType.String, model.Skype);
                db.AddInParameter("@yahoo", DbType.String, model.Yahoo);
                db.AddInParameter("@mariage", DbType.Boolean, model.Mariage);
                db.AddInParameter("@address", DbType.String, model.Address);
                db.AddInParameter("@currAddress", DbType.String, model.CurrentAddress);
                db.AddInParameter("@idCardNo", DbType.String, model.CardNo);
                db.AddInParameter("@passportNo", DbType.String, model.PassportNo);
                db.AddInParameter("@passportExp", DbType.DateTime, model.PassportExp);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@modifiedBy", DbType.Int32, model.Modifier.Id);
                db.AddInParameter("@password", DbType.String, model.Password);
                try
                {
                    return db.ExecuteNonQuery() != 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        public EmployeeShift GetEmployeeShift(int id)
        {
            using (var db = new ObjectDb("EmployeeShift_Get"))
            {
                return db.Query<EmployeeShift>(new { id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Method update Employee's shift.
        /// </summary>
        /// <param name="model">EmployeeShift model.</param>
        /// <returns>Boolean to check updated status.</returns>
        public bool UpdateEmployeeShift(EmployeeShift model)
        {
            using (var db = new ObjectDb("EmployeeShift_Update"))
            {
                db.AddInParameter("@id", DbType.Int32, model.EmpID);
                db.AddInParameter("@monShift", DbType.Int32, model.MonShift);
                db.AddInParameter("@tueShift", DbType.Int32, model.TueShift);
                db.AddInParameter("@wedShift", DbType.Int32, model.WedShift);
                db.AddInParameter("@thuShift", DbType.Int32, model.ThuShift);
                db.AddInParameter("@friShift", DbType.Int32, model.FriShift);
                db.AddInParameter("@satShift", DbType.Int32, model.SatShift);
                db.AddInParameter("@sunShift", DbType.Int32, model.SunShift);
                return db.ExecuteNonQuery() != 0;
            }
        }

        public IList<WeekWorking> GetWeekWorking(int id, DateTime date)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetWeekWorking"))
            {
                return db.Query<WeekWorking>(new { id, date }).ToList();
            }
        }

        /// <summary>
        /// Method update week working.
        /// </summary>
        /// <param name="model">EmployeeShift model.</param>
        /// <param name="date">Date time for updating.</param>
        /// <returns>Boolean to check status.</returns>
        public bool UpdateWeekWorking(EmployeeShift model, DateTime date)
        {
            using (var db = new ObjectDb("EmployeeCalendar_UpdateWeekWorking"))
            {
                db.AddInParameter("@id", DbType.Int32, model.EmpID);
                db.AddInParameter("@date", DbType.DateTime, date);
                db.AddInParameter("@monShift", DbType.Int32, model.MonShift);
                db.AddInParameter("@tueShift", DbType.Int32, model.TueShift);
                db.AddInParameter("@wedShift", DbType.Int32, model.WedShift);
                db.AddInParameter("@thuShift", DbType.Int32, model.ThuShift);
                db.AddInParameter("@friShift", DbType.Int32, model.FriShift);
                db.AddInParameter("@satShift", DbType.Int32, model.SatShift);
                db.AddInParameter("@sunShift", DbType.Int32, model.SunShift);
                return db.ExecuteNonQuery() != 0;
            }
        }

        public MySpaceViewModel GetMySpace(int empid)
        {
            using (var db = new ObjectDb("Report_EmployeeDashBoard"))
            {
                var date = DateTime.Now.ToShortDateString();
                return db.Query<MySpaceViewModel>(new { empid, date }).FirstOrDefault();
            }
        }

        public HrLeaderDashboardModel GetHrLeaderDashboard(DateTime date, int? employeeId = null)
        {
            using (var db = new ObjectDb("Report_HRLeaderDashBoard"))
            {
                return db.Query<HrLeaderDashboardModel>(new { employeeId, date }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Method get working schedule.
        /// </summary>
        /// <param name="date">Working schedule's date time.</param>
        /// <param name="id">Employee's id.</param>
        /// <returns>A list of working schedule model.</returns>
        public IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id)
        {
            using (var db = new ObjectDb("Report_EmployeeCalendarByMonth"))
            {
                var ds = new DataSet();

                db.AddInParameter("@reportDate", DbType.DateTime, date);
                db.AddInParameter("@employeeId", DbType.Int32, id);
                ds = db.ExecuteDataSet();
                var dttWorkingSchedule = ds != null ? ds.Tables[0] : null;

                var list = new List<WorkingScheduleModel>();
                if (dttWorkingSchedule.Rows.Count != 0)
                {
                    for (int i = 1; i <= dttWorkingSchedule.Columns.Count - 4; i++)
                    {
                        var workingDay = new WorkingScheduleModel
                        {
                            Title = dttWorkingSchedule.Rows[0][i + 3].ToString(),
                            Start = new DateTime(date.Year, date.Month, i).ToShortDateString()
                        };
                        list.Add(workingDay);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Method get working calendars.
        /// </summary>
        /// <param name="date">Date time of working calendar.</param>
        /// <returns>A list of working calendar ViewModel.</returns>
        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(string date)
        {
            using (var db = new ObjectDb("Report_EmployeeCalendarByMonth"))
            {
                var list = new List<WorkingCalendarsViewModel>();
                var ds = new DataSet();

                db.AddInParameter("@reportDate", DbType.DateTime, date);
                ds = db.ExecuteDataSet();
                var dttWorkingCalendars = ds != null ? ds.Tables[0] : null;
                var totalColumns = dttWorkingCalendars.Columns.Count;

                foreach (System.Data.DataRow row in dttWorkingCalendars.Rows)
                {
                    var empWorkingCalendar = new WorkingCalendarsViewModel();

                    empWorkingCalendar.EmpId = int.Parse(row["Empid"].ToString());
                    empWorkingCalendar.FullName = row["fullname"].ToString();
                    empWorkingCalendar.NickName = row["nickname"].ToString();
                    empWorkingCalendar.Team = row["Team"].ToString();
                    empWorkingCalendar.Days = totalColumns - 4;
                    List<string> days = new List<string>();
                    for (int i = 4; i < totalColumns; i++)
                    {
                        days.Add(row[i].ToString());
                    }

                    empWorkingCalendar.WorkingDays = days;

                    list.Add(empWorkingCalendar);
                }

                return list;
            }
        }

        /// <summary>
        /// Method update shift.
        /// </summary>
        /// <param name="id">Id of the Shift.</param>
        /// <param name="date">Date time to update.</param>
        /// <param name="shift">Shift number.</param>
        /// <param name="modifiedBy">Modified by.</param>
        /// <param name="modifiedDate">Modified date.</param>
        /// <returns>Boolean to check status.</returns>
        public bool UpdateShift(int id, DateTime date, int? shift, int modifiedBy, DateTime modifiedDate)
        {
            using (ObjectDb db = new ObjectDb("EmployeeCalendar_Update"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, id);
                db.AddInParameter("@date", DbType.DateTime, date);
                db.AddInParameter("@ShiftID", DbType.Int32, shift);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);
                return db.ExecuteNonQuery() != 0;
            }
        }

        public IList<Employee> GetMembersByManager(int managerId, short role, string keyword, int? groupId = null)
        {
            int excludedManager = 0;
            using (ObjectDb db = new ObjectDb("Employee_GetMembesByManager"))
            {
                return db.Query<Employee>(new { managerId, role, excludedManager, groupId, keyword }).ToList();
            }
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
            using (ObjectDb db = new ObjectDb("AnnualLeave_InsertMultipleTransactions"))
            {
                return db.ExecuteNonQuery(new { empIds, description, fromDate, deductDays, creditDays, createdDate, createdBy }) != 0;
            }
        }

        public bool ResetAnnualLeaves(string empValue, string description, DateTime fromDate, int resetDays, DateTime createdDate, int createdBy)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_ResetMultipleTransactions"))
            {
                return db.ExecuteNonQuery(new { empValue, description, fromDate, resetDays, createdDate, createdBy }) != 0;
            }
        }

        public IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_GetTransByEmp"))
            {
                return db.Query<AnnualLeaveBalanceModel>(new { empId = id, year, excludedRequestTrans }).ToList();
            }
        }

        public bool DeleteAnnualLeaveTransaction(long id, DateTime modifiedDate, int modifiedBy)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_DeleteTransaction"))
            {
                return db.ExecuteNonQuery(new { id, modifiedDate, modifiedBy }) != 0;
            }
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_GetByID"))
            {
                return db.Query<AnnualLeaveBalanceModel>(new { id }).SingleOrDefault();
            }
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
            using (ObjectDb db = new ObjectDb("AnnualLeave_UpdateTransaction"))
            {
                return db.ExecuteNonQuery(new { id, description, fromDate, deductDays, creditDays, modifiedDate, modifiedBy }) != 0;
            }
        }

        /// <summary>
        /// Method insert annual leave.
        /// </summary>
        /// <param name="empId">Employee's id.</param>
        /// <param name="description">Description to leave.</param>
        /// <param name="fromDate">Leave from date.</param>
        /// <param name="deductDays">Deduct days.</param>
        /// <param name="creditDays">Credit days.</param>
        /// <param name="createdDate">Created date.</param>
        /// <param name="createdBy">Created by.</param>
        /// <returns>Boolean to check status.</returns>
        public bool InsertAnnualLeaveTransaction(
            int empId,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_InsertTransaction"))
            {
                return db.ExecuteNonQuery(new
                {
                    empId,
                    description,
                    fromDate,
                    deductDays,
                    creditDays,
                    createdDate,
                    createdBy
                }) != 0;
            }
        }

        public IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees()
        {
            using (ObjectDb db = new ObjectDb("GetMissingCheckInOutList"))
            {
                return db.Query<MissingCheckInOutModel>().ToList();
            }
        }

        public IList<UserChangedShift> GetUserChangedShift(DateTime reportDate)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetChangedShift"))
            {
                return db.Query<UserChangedShift>(new { reportDate }).ToList();
            }
        }

        public IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetHalfDayOff"))
            {
                return db.Query<HalfDayOff>(new { reportDate, employeeId, groupId }).ToList();
            }
        }

        /// <summary>
        /// Method update off day for all employee.
        /// </summary>
        /// <param name="date">Off day's date time.</param>
        /// <param name="isOffDay">Is off day.</param>
        /// <param name="modifiedBy">Modified by.</param>
        /// <param name="modifiedDate">Modified Date.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Updating message.</param>
        /// <returns>Boolean to check status.</returns>
        public bool UpdateOffDayForAllEmployees(
            DateTime date,
            int isOffDay,
            int modifiedBy,
            DateTime modifiedDate,
            out int errorCode,
            out string message)
        {
            using (var db = new ObjectDb("EmployeeCalendar_SetOffDayForAllEmployees"))
            {
                db.AddInParameter("@date", DbType.DateTime, date);
                db.AddInParameter("@isOffDay", DbType.Int16, isOffDay);
                db.AddInParameter("@ModifiedBy", DbType.Int16, modifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                var requestId = result == null ? -1 : (long)result;
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return true;
            }
        }

        /// <summary>
        /// Method get Range date.
        /// </summary>
        /// <param name="employeeId">Employee's id.</param>
        /// <param name="fromDate">Range from date.</param>
        /// <param name="toDate">Range to date.</param>
        /// <returns>Number days.</returns>
        public int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            int numberDay = 0;
            if (toDate.Date >= fromDate.Date)
            {
                using (var db = new ObjectDb("EmployeeCalendar_GetRangeDate"))
                {
                    var ds = new DataSet();

                    db.AddInParameter("@EmpId", DbType.Int32, employeeId);
                    db.AddInParameter("@fromDate", DbType.DateTime, fromDate);
                    db.AddInParameter("@toDate", DbType.DateTime, toDate);

                    ds = db.ExecuteDataSet();
                    var dataTableRangeDate = ds != null ? ds.Tables[0] : null;

                    if (dataTableRangeDate != null)
                    {
                        if (dataTableRangeDate.Rows.Count > 0)
                        {
                            numberDay = int.Parse(dataTableRangeDate.Rows[0][0].ToString());
                        }
                    }
                }
            }

            return numberDay;
        }

        public IEnumerable<DateTime> GetDisableRequestDate(int empId, DateTime fromDate, DateTime toDate)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetUnavailRequestDate"))
            {
                var unavailableDates = db.Query<DateTime>(new { empId = empId, fromDate = fromDate, toDate = toDate });
                return unavailableDates;
            }
        }

        /// <summary>
        /// Method change password.
        /// </summary>
        /// <param name="empId">Employee's id.</param>
        /// <param name="password">Current password.</param>
        /// <returns>An number to check status.</returns>
        public int ChangePassword(int empId, string password)
        {
            int effectedRow = -1;
            using (var db = new ObjectDb("Employee_ChangePassword"))
            {
                db.AddInParameter("@empId", DbType.Int32, empId);
                db.AddInParameter("@password", DbType.String, password);

                try
                {
                    db.ExecuteNonQuery();
                    effectedRow = 0;
                }
                catch
                {
                }
            }

            return effectedRow;
        }
    }
}
