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
    }
}
