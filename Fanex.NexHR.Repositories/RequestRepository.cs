// --------------------------------------------------------------------------
// <copyright file="RequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.9">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Request Repository.
    /// </summary>
    public class RequestRepository : IRequestRepository
    {
        /// <summary>
        /// Method Request Working Time.
        /// </summary>
        /// <param name="empId">Employee Id.</param>
        /// <param name="requestType">Request type.</param>
        /// <param name="isPlanned">Is Planned.</param>
        /// <param name="remark">Remark, comment.</param>
        /// <param name="createdDate">Created date.</param>
        /// <param name="createdBy">Created by.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Message to request.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromShift">From shift.</param>
        /// <param name="toDate">Request to Date.</param>
        /// <param name="toShift">Request to shift.</param>
        /// <param name="leaveTypeId">Leave type id.</param>
        /// <param name="totalDays">Total days.</param>
        /// <param name="lateInMins">Late in minutes.</param>
        /// <param name="earlyOutMins">Early out minutes.</param>
        /// <param name="overtimeType">Overtime type.</param>
        /// <param name="isOvertimeHours">Check isOverTime hours.</param>
        /// <returns>Request id.</returns>
        public long WorkingTimeRequest(
            int empId,
            short requestType,
            bool isPlanned,
            string remark,
            DateTime createdDate,
            int createdBy,
            out int errorCode,
            out string message,
            DateTime fromDate,
            int? fromShift = null,
            DateTime? toDate = null,
            int? toShift = null,
            int? leaveTypeId = null,
            decimal? totalDays = null,
            int? lateInMins = null,
            int? earlyOutMins = null,
            short? overtimeType = null,
            bool isOvertimeHours = false)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_Insert"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, empId);
                db.AddInParameter("@RequestType", DbType.Int16, requestType);
                db.AddInParameter("@IsPlanned", DbType.Boolean, isPlanned);
                db.AddInParameter("@Remark", DbType.String, remark);

                db.AddInParameter("@FromDate", DbType.DateTime, fromDate);
                db.AddInParameter("@FromShift", DbType.Int32, fromShift);
                db.AddInParameter("@ToDate", DbType.DateTime, toDate);
                db.AddInParameter("@ToShift", DbType.Int32, toShift);

                db.AddInParameter("@LeaveTypeID", DbType.Int32, leaveTypeId);
                db.AddInParameter("@TotalDays", DbType.Decimal, totalDays);
                db.AddInParameter("@LateInMins", DbType.Int32, lateInMins);
                db.AddInParameter("@EarlyOutMins", DbType.Int32, earlyOutMins);

                db.AddInParameter("@OvertimeType", DbType.Int16, overtimeType);
                db.AddInParameter("@IsOvertimeHours", DbType.Boolean, isOvertimeHours);

                db.AddInParameter("@CreatedDate", DbType.DateTime, createdDate);
                db.AddInParameter("@CreatedBy", DbType.Int32, createdBy);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                var requestId = result == null ? -1 : (long)result;
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return requestId;
            }
        }

        /// <summary>
        /// Method update request.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isPlanned">Is Planned.</param>
        /// <param name="remark">Remark, comment.</param>
        /// <param name="modifiedby">Modified by.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="modifiedDate">Modified date.</param>
        /// <param name="fromShift">From Shift.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="toShift">Request to shift.</param>
        /// <param name="leaveTypeId">Leave type id.</param>
        /// <param name="totalDays">Total days.</param>
        /// <param name="lateInMins">Late in minutes.</param>
        /// <param name="earlyOutMins">Early out minutes.</param>
        /// <param name="overtimeType">Overtime type.</param>
        /// <param name="isOvertimeHours">Is overtime hours.</param>
        /// <returns>Boolean to check status.</returns>
        public bool UpdateRequest(
           long requestId,
           bool isPlanned,
           string remark,
           int modifiedby,
           DateTime fromDate,
           DateTime modifiedDate,
           int? fromShift = null,
           DateTime? toDate = null,
           int? toShift = null,
           int? leaveTypeId = null,
           decimal? totalDays = null,
           int? lateInMins = null,
           int? earlyOutMins = null,
           short? overtimeType = null,
           bool isOvertimeHours = false)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_Update"))
            {
                db.AddInParameter("@Id", DbType.Int64, requestId);
                db.AddInParameter("@IsPlanned", DbType.Boolean, isPlanned);
                db.AddInParameter("@Remark", DbType.String, remark);
                db.AddInParameter("@FromDate", DbType.DateTime, fromDate);
                db.AddInParameter("@FromShift", DbType.Int32, fromShift);
                db.AddInParameter("@ToDate", DbType.DateTime, toDate);
                db.AddInParameter("@ToShift", DbType.Int32, toShift);
                db.AddInParameter("@LeaveTypeID", DbType.Int32, leaveTypeId);
                db.AddInParameter("@TotalDays", DbType.Decimal, totalDays);
                db.AddInParameter("@LateInMins", DbType.Int32, lateInMins);
                db.AddInParameter("@EarlyOutMins", DbType.Int32, earlyOutMins);

                db.AddInParameter("@OvertimeType", DbType.Int16, overtimeType);
                db.AddInParameter("@IsOvertimeHours", DbType.Boolean, isOvertimeHours);

                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedby);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);
                return db.ExecuteNonQuery() != 0;
            }
        }

        public bool DeleteWorkingTimeRequest(int id)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_Delete"))
            {
                db.AddInParameter("@id", DbType.Int32, id);
                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Method process request.
        /// </summary>
        /// <param name="command">Commands (approve, reject..).</param>
        /// <param name="requestId">Request id.</param>
        /// <param name="processedBy">Processed by.</param>
        /// <param name="processedDate">Processed date.</param>
        /// <param name="message">Message comment.</param>
        /// <returns>Number to check status.</returns>
        public int ProcessRequest(
            Commands command,
            long requestId,
            int processedBy,
            DateTime processedDate,
            out string message)
        {
            string key = string.Empty;
            string processedByKey = string.Empty;
            string processedDateKey = string.Empty;
            switch (command)
            {
                case Commands.Approve:
                    key = "WorkingTimeRequest_ApproveRequest";
                    processedByKey = "@ApprovedBy";
                    processedDateKey = "@ApprovedDate";
                    break;
                case Commands.Disapprove:
                    key = "WorkingTimeRequest_DisapproveRequest";
                    processedByKey = "@ModifiedBy";
                    processedDateKey = "@ModifiedDate";
                    break;
                case Commands.Reject:
                    key = "WorkingTimeRequest_RejectRequest";
                    processedByKey = "@ProcessedBy";
                    processedDateKey = "@ProcessedDate";
                    break;
            }

            using (ObjectDb db = new ObjectDb(key))
            {
                db.AddInParameter("@RequestID", DbType.Int64, requestId);

                db.AddInParameter(processedByKey, DbType.Int32, processedBy);
                db.AddInParameter(processedDateKey, DbType.DateTime, processedDate);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                message = db.GetParameterValue("ErrMsg") as string;
                return (int)db.GetParameterValue("ErrCode");
            }
        }

        public IList<WorkingTimeRequest> GetWorkingTimeRequests(int employeeId, int? requestType = null, byte? status = null)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_GetByEmployee"))
            {
                return db.Query<WorkingTimeRequest>(new { id = employeeId, requestType, status }).ToList();
            }
        }

        public WorkingTimeRequest GetWorkingTimeRequest(long id)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_Get"))
            {
                return db.Query<WorkingTimeRequest>(new { id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Method Get all request.
        /// </summary>
        /// <param name="roleId">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Get to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is Daily request.</param>
        /// <param name="employeeId">Employee id.</param>
        /// <param name="status">Request status.</param>
        /// <param name="logInId">Log in id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetAllRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest ? "WorkingTimeRequest_GetRequestByDate_AllType" : "WorkingTimeRequest_GetRequestByEmp_AllType";
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = roleId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"), CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                Requester = new Employee
                                {
                                    Id = reader.To<int>("empID"),
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate"),
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                                TotalMins = reader.ToInt("Time"),
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                LeaveType = reader.ToString("LeaveType"),
                                Remark = reader.ToString("Remark"),
                                OvertimeType = reader.ToShort("OvertimeType"),
                                IsOvertimeHours = reader.ToBool("IsOvertimeHours")
                            };
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method get leave request.
        /// </summary>
        /// <param name="roleId">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is daily request.</param>
        /// <param name="employeeId">Employee's id.</param>
        /// <param name="status">Request's status.</param>
        /// <param name="logInId">Log in id.</param>
        /// <returns>List RequestEntity.</returns>
        public IList<RequestEntity> GetLeaveRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest
                ? "WorkingTimeRequest_GetRequestByDate_LeaveType"
                : "WorkingTimeRequest_GetRequestByEmp_LeaveType";
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = roleId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                LeaveType = reader.ToString("LeaveType"),
                                Remark = reader.ToString("Remark"),
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method get change shift request.
        /// </summary>
        /// <param name="roleId">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is daily request.</param>
        /// <param name="employeeId">Employee's id.</param>
        /// <param name="status">Request's status.</param>
        /// <param name="logInId">Log in id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetChangeShiftRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest
                ? "WorkingTimeRequest_GetRequestByDate_ChangeShiftType"
                : "WorkingTimeRequest_GetRequestByEmp_ChangeShiftType";

            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = roleId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                                Remark = reader.ToString("Remark"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method get late in early out request.
        /// </summary>
        /// <param name="roleId">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is daily request.</param>
        /// <param name="employeeId">Employee's id.</param>
        /// <param name="status">Request's status.</param>
        /// <param name="logInId">Log in id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetLateInEarlyOutRequest(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest
                ? "WorkingTimeRequest_GetRequestByDate_LateInEarlyOutType"
                : "WorkingTimeRequest_GetRequestByEmp_LateInEarlyOutType";

            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = roleId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.ToString("Shift") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalMins = reader.ToInt("Time"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Remark = reader.ToString("Remark"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method get check in out request.
        /// </summary>
        /// <param name="roleId">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is daily request.</param>
        /// <param name="employeeId">Employee's id.</param>
        /// <param name="status">Request's status.</param>
        /// <param name="logInId">Log in id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetCheckInOutRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest
                ? "WorkingTimeRequest_GetRequestByDate_CheckinOutType"
                : "WorkingTimeRequest_GetRequestByEmp_CheckinOutType";
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = roleId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.ToString("Shift") },
                                RequestedDate = reader.ToDateTime("RequestTime"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method GetOvertimeRequest.
        /// </summary>
        /// <param name="roleId">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is daily request.</param>
        /// <param name="employeeId">Employee's id.</param>
        /// <param name="status">Request's status.</param>
        /// <param name="logInId">Log in id.</param>
        /// <returns>List RequestEntity.</returns>
        public IList<RequestEntity> GetOvertimeRequest(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest ? "WorkingTimeRequest_GetRequestByDate_OvertimeType" : "WorkingTimeRequest_GetRequestByEmp_OvertimeType";
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = roleId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                Remark = reader.ToString("Remark"),
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                                OvertimeType = reader.ToShort("OvertimeType"),
                                IsOvertimeHours = reader.ToBool("IsOvertimeHours"),
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },

                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method GetLeaveRequestHistory.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type id.</param>
        /// <returns>List RequestEntity.</returns>
        public IList<RequestEntity> GetLeaveRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                LeaveType = reader.ToString("LeaveType"),
                                Remark = reader.ToString("Remark"),

                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method GetChangeShiftRequestHistory.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <param name="requestTypeId">Request type id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetChangeShiftRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Remark = reader.ToString("Remark"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                RequestedDate = reader.ToDateTime("RequestDate"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method Get Late In Early Out Request History.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request Type Id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetLateInEarlyOutRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.ToString("Shift") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalMins = reader.ToInt("Time"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                Remark = reader.ToString("Remark"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method Get Check In Out Request History.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request Type Id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetCheckInOutRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.ToString("Shift") },
                                RequestedDate = reader.ToDateTime("RequestTime"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method Get OvertimeRequestHistory.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <param name="requestTypeId">Request type id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetOvertimeRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                Remark = reader.ToString("Remark"),
                                OvertimeType = reader.ToShort("OvertimeType"),
                                IsOvertimeHours = reader.ToBool("IsOvertimeHours"),
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },

                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }
    }
}
