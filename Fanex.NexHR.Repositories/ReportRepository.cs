// --------------------------------------------------------------------------
// <copyright file="ReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report Repository.
    /// </summary>
    public class ReportRepository : IReportRepository
    {
        /// <summary>
        /// Method get Employee Attendance report.
        /// </summary>
        /// <param name="fromTime">Get from time.</param>
        /// <param name="toTime">Get to time.</param>
        /// <param name="type">Type to get.</param>
        /// <param name="empId">Employee's id.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="excludeUserDelete">Exclude user deleted.</param>
        /// <returns>List of EmployeeAttendanceModel.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(
            DateTime fromTime,
            DateTime toTime,
            short type,
            int? empId = null,
            string groupIds = null,
            int? excludeUserDelete = 1)
        {
            var listAttendance = new List<EmployeeAttendanceModel>();
            EmployeeAttendanceModel currentAttendance = new EmployeeAttendanceModel();
            EmployeeAttendanceRequest request = new EmployeeAttendanceRequest();
            EmployeeAttendanceOvertime overtime = new EmployeeAttendanceOvertime();

            using (ObjectDb objectDb = new ObjectDb("EmployeeAttendance_GetByType"))
            {
                objectDb.Query<EmployeeAttendanceModel, dynamic, EmployeeAttendanceModel>(
                    (attendance, t) =>
                    {
                        if (currentAttendance == null || currentAttendance.Id != attendance.Id)
                        {
                            listAttendance.Add(attendance);
                            currentAttendance = attendance;
                            currentAttendance.RequestList = new List<EmployeeAttendanceRequest>();
                            currentAttendance.OvertimeList = new List<EmployeeAttendanceOvertime>();
                        }

                        if (t != null && t.RequestType != null)
                        {
                            request = new EmployeeAttendanceRequest
                            {
                                RequestType = new RequestType { Id = t.RequestType },
                                FromDate = t.FromDate,
                                ToDate = t.ToDate,
                                Remark = t.Remark,
                                TotalDays = t.TotalDays,
                                LeaveType = t.LeaveType,
                                FromShift = t.FromShift,
                                ToShift = t.ToShift,
                                TotalMins = t.Time,
                                OvertimeType = t.OvertimeType != null ? new OvertimeType { Id = t.OvertimeType } : new OvertimeType(),
                                IsOvertimeHours = t.OvertimeType != null ? t.IsOvertimeHours : false
                            };
                            currentAttendance.RequestList.Add(request);

                            if (request.RequestType.Id == (short)RequestTypes.Overtime 
                                && t.OvertimeName != null 
                                && (t.OvertimeFromTime != null || t.OvertimeToTime != null))
                            {
                                overtime = new EmployeeAttendanceOvertime 
                                {
                                    OvertimeName = t.OvertimeName,
                                    OvertimeType = t.OvertimeType != null ? new OvertimeType { Id = t.OvertimeType } : new OvertimeType(),
                                    OvertimeFromTime = t.OvertimeFromTime,
                                    OvertimeToTime = t.OvertimeToTime,
                                    OvertimeTotalHours = t.OvertimeTotalHours
                                };
                                currentAttendance.OvertimeList.Add(overtime);
                            }
                        }

                        return currentAttendance;
                    },
                    "RequestType",
                    new { fromTime, toTime, type, empId, groupIds, excludeUserDelete });
            }

            return listAttendance;
        }

        /// <summary>
        /// Method GetRequest Monitoring.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Get to date.</param>
        /// <param name="maxRequest">Max Request.</param>
        /// <param name="role">Role of employee.</param>
        /// <param name="employeeID">Employee's Id.</param>
        /// <returns>List of RequestMonitoring.</returns>
        public IList<RequestsMonitoring> GetRequestMonitoring(DateTime fromDate, DateTime toDate, int maxRequest, int role, int employeeID)
        {
            using (var db = new ObjectDb("RequestMonitoring"))
            {
                var entityList = new List<RequestsMonitoring>();

                using (IDataReader reader = db.ExecuteReader(new { fromDate, toDate, maxRequest, role, employeeID }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestsMonitoring
                            {
                                Requester = new Employee
                                {
                                    FullName = reader.ToString("FullName"),
                                    NickName = reader.ToString("NickName"),
                                    Team = new Team { Name = reader.ToString("Team"), Id = reader.ToInt("TeamId") },
                                    Id = reader.ToInt("EmpID")
                                },
                                TotalCheckInOut = reader.ToInt("TotalCheckInOutRequest"),
                                TotalLateInEarlyOut = reader.ToInt("TotalLateInEarlyOutRequest"),
                                TotalChangeShift = reader.ToInt("TotalChangeShiftRequest"),
                                TotalLeave = reader.ToInt("TotalLeaveRequest"),
                                TotalOvertime = reader.ToInt("TotalOvertimeRequest")
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        /// <summary>
        /// Method get working hours.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Get to date.</param>
        /// <param name="empId">Employee's id.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <returns>List of TotalWorkingHours.</returns>
        public IList<TotalWorkingHours> GetWorkingHoursList(DateTime fromDate, DateTime toDate, int? empId, string groupIds)
        {
            using (var db = new ObjectDb("Report_WorkingHoursSummary"))
            {
                var entityList = new List<TotalWorkingHours>();

                using (IDataReader reader = db.ExecuteReader(new { fromDate, toDate, empId, groupIds }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new TotalWorkingHours
                            {
                                Employee = new Employee
                                {
                                    Code = reader.ToString("Code"),
                                    FullName = reader.ToString("FullName"),
                                    NickName = reader.ToString("NickName"),
                                    Team = new Team { Name = reader.To<string>("Team") },
                                    Id = reader.ToInt("empid")
                                },
                                WorkingDay = reader.ToInt("WorkingDays"),
                                ActualWorkingDay = reader.ToFloat("ActualWorkingDays"),
                                LeaveTaken = reader.ToFloat("LeaveTaken"),
                                UnpaidLeave = reader.ToFloat("UnpaidLeave"),
                                LateInMins = reader.ToInt("LateIn"),
                                EarlyOutMins = reader.ToInt("EarlyOut"),
                                TotalHours = reader.ToFloat("TotalWorkHours"),
                                AverageHours = reader.ToFloat("AverageHours")
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        /// <summary>
        /// Method get annual leave balance.
        /// </summary>
        /// <param name="year">Year to get.</param>
        /// <param name="empId">Employee's id.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <returns>List of LeaveSummary.</returns>
        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(int year, int? empId, string groupIds)
        {
            using (var db = new ObjectDb("Report_LeaveSummary"))
            {
                var entityList = new List<LeaveSummary>();

                using (IDataReader reader = db.ExecuteReader(new { empId, groupIds, year }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new LeaveSummary
                            {
                                Employee = new Employee
                                {
                                    FullName = reader.ToString("FullName"),
                                    NickName = reader.ToString("NickName"),
                                    Team = new Team { Name = reader.To<string>("Team") },
                                    Id = reader.ToInt("empid"),
                                    JoinedDate = reader.ToDateTime("JoinedDate"),
                                    ConfirmedDate = reader.ToDateTime("ConfirmedDate")
                                },
                                AnnualLeaveLastYear = reader.ToFloat("AnnualLeavelastYear"),
                                AnnualLeaveCurrentYear = reader.ToFloat("AnnualLeaveCurrentYear"),
                                Jan = reader.ToFloat("Jan"),
                                Feb = reader.ToFloat("Feb"),
                                Mar = reader.ToFloat("Mar"),
                                Apr = reader.ToFloat("Apr"),
                                May = reader.ToFloat("May"),
                                Jun = reader.ToFloat("Jun"),
                                Jul = reader.ToFloat("Jul"),
                                Aug = reader.ToFloat("Aug"),
                                Sep = reader.ToFloat("Sep"),
                                Oct = reader.ToFloat("Oct"),
                                Nov = reader.ToFloat("Nov"),
                                Dec = reader.ToFloat("Dec"),
                                LeaveBalance = reader.ToFloat("LeaveBalance"),
                                Remark = reader.ToString("Remark")
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        public IList<ExtensionNoModel> GetAllExtensionNo()
        {
            using (var db = new ObjectDb("ExtensionNO_GetAll"))
            {
                var lstEntity = db.Query<ExtensionNoModel>().ToList();
                return lstEntity;
            }
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(bool isInternal, int? groupId)
        {
            using (var db = new ObjectDb("ExtensionNO_GetGroupInfo"))
            {
                var lstEntity = db.Query<ExtensionNoModel>(new { isInternal = isInternal, GroupID = groupId }).ToList();
                return lstEntity;
            }
        }

        /// <summary>
        /// Method add new Extension Number member.
        /// </summary>
        /// <param name="model">ExtensionNoModel model.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check result.</returns>
        public bool AddNewExtensionNoMember(ExtensionNoModel model, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("ExtensionNO_Insert_EmployeeInfo"))
            {
                db.AddInParameter("@FullName", DbType.String, model.FullName);
                db.AddInParameter("@NickName", DbType.String, model.NickName);
                db.AddInParameter("@ExtensionNo", DbType.String, model.ExtensionNo);

                db.AddInParameter("@IsInternal", DbType.Boolean, model.IsInternal);
                db.AddInParameter("@EmpID", DbType.Int32, model.EmpId);
                db.AddInParameter("@GroupID", DbType.Int32, model.TeamId);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                if (errorCode != 0)
                {
                    return false;
                }

                return true;
            }
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(bool isInternal, int empExtID)
        {
            IList<ExtensionNoModel> entityList = new List<ExtensionNoModel>();
            using (var db = new ObjectDb("ExtensionNO_GetEmployeeInfo"))
            {
                var entity = db.QueryEntity<ExtensionNoModel>(new { isInternal = isInternal, EmpExtID = empExtID });

                return entity;
            }
        }

        /// <summary>
        /// Method update Extension Number member.
        /// </summary>
        /// <param name="model">ExtensionNoModel model.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check Result.</returns>
        public bool UpdateExtensionNoMember(ExtensionNoModel model, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("ExtensionNO_Update_EmployeeInfo"))
            {
                db.AddInParameter("@FullName", DbType.String, model.FullName);
                db.AddInParameter("@NickName", DbType.String, model.NickName);
                db.AddInParameter("@ExtensionNo", DbType.String, model.ExtensionNo);

                db.AddInParameter("@IsInternal", DbType.Boolean, model.IsInternal);
                db.AddInParameter("@ExtID", DbType.Int32, model.EmpExtID);
                db.AddInParameter("@EmpID", DbType.Int32, model.EmpId);
                db.AddInParameter("@GroupID", DbType.Int32, model.TeamId);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                if (errorCode != 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Method Delete ExtensionNumber.
        /// </summary>
        /// <param name="extId">Extension Id.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool DeleteExtensionNumber(int extId, out int errorCode, out string message)
        {
            using (ObjectDb db = new ObjectDb("ExtensionNO_DELETE_EmployeeInfo"))
            {
                db.AddInParameter("@ExtID", DbType.Int32, extId);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteScalar();

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");
                if (errorCode != 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Method Add New External Group.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool AddNewExternalGroup(string groupName, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("ExtensionNO_Insert_GroupExternal"))
            {
                db.AddInParameter("@FullName", DbType.String, groupName);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                if (errorCode != 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Method Update an External Group.
        /// </summary>
        /// <param name="groupId">Group's id.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool EditExternalGroup(int groupId, string groupName, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("ExtensionNO_Update_GroupExternal"))
            {
                db.AddInParameter("@FullName", DbType.String, groupName);
                db.AddInParameter("@ExtID", DbType.String, groupId);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                if (errorCode != 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Method Delete external group.
        /// </summary>
        /// <param name="groupId">Group's Id.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool DeleteExternalGroup(int groupId, out int errorCode, out string message)
        {
            using (ObjectDb db = new ObjectDb("ExtensionNO_DELETE_GroupExternal"))
            {
                db.AddInParameter("@ExtID", DbType.Int32, groupId);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteScalar();

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");
                if (errorCode != 0)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
