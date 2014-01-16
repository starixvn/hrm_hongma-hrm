// --------------------------------------------------------------------------
// <copyright file="ReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.20">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report Service.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService()
        {
            this._reportRepository = new ReportRepository();
        }

        public IList<TotalWorkingHours> GetWorkingHoursList(
            DateTime fromDate,
            DateTime toDate,
            int? empId,
            string groupIds) 
        {
            var totalWorkingHours = _reportRepository.GetWorkingHoursList(fromDate, toDate, empId, groupIds);

            return totalWorkingHours;
        }

        public IList<RequestsMonitoring> GetRequestMonitoring(
            DateTime fromDate,
            DateTime toDate,
            int maxRequest,
            int role,
            int employeeID)
        {
            var requestMonitoring = _reportRepository.GetRequestMonitoring(fromDate, toDate, maxRequest, role, employeeID);

            return requestMonitoring;
        }

        /// <summary>
        /// Function to get employeeAttendance Report.
        /// </summary>
        /// <param name="fromTime">From time to query DB.</param>
        /// <param name="toTime">To time to query DB..</param>
        /// <param name="type">Type of request.</param>
        /// <param name="empId">Employee Id.</param>
        /// <param name="groupIds">Group list.</param>
        /// <param name="excludeUserDelete">Is exclude user deleted.</param>
        /// <returns>EmployeeAttendanceModel List object.</returns>
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(
            DateTime fromTime,
            DateTime toTime,
            short type,
            int? empId = null,
            string groupIds = null,
            int? excludeUserDelete = 1)
        {
            var employeeAttendance = _reportRepository.GetEmployeeAttendanceReport(
                fromTime, 
                toTime,
                type,
                empId,
                groupIds,
                excludeUserDelete);

            return employeeAttendance;
        }

        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(
            int year,
            int? empId,
            string groupIds)
        {
            var annualLeaveBalance = _reportRepository.GetAnnualLeaveBalanceReport(year, empId, groupIds);

            return annualLeaveBalance;
        }

        #region Get leave summary by team

        /// <summary>
        /// This function use for sorting leave summaries. Any leave summary of employee will be grouped by team. (For easier displaying in view).
        /// </summary>
        /// <param name="leaveSummaryList">Leave summary List.</param>
        /// <param name="teams">Team List.</param>
        /// <returns>List Annual Leave.</returns>        
        public IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> teams)
        {
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = new List<LeaveSummaryByTeam>();
            if (teams.Count > 1 || (teams.Count == 1 && teams[0].Id == 11))
            {
                var teamService = new TeamService();
                LeaveSummaryByTeam management = new LeaveSummaryByTeam
                {
                    Team = teamService.Get(11),
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                // Add management team in 1st position
                leaveSummaryByTeamList.Add(management);
            }

            foreach (Team team in teams)
            {
                LeaveSummaryByTeam entity = new LeaveSummaryByTeam
                {
                    Team = team,
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                foreach (LeaveSummary leavesumary in leaveSummaryList)
                {
                    if (leavesumary.Employee.Team.Name == team.Name)
                    {
                        /*Check if team is mamagement team, add leave summary for them*/
                        if ((team.Id == 11 && teams.Count > 1) || (teams.Count == 1 && teams[0].Id == 11))
                        {
                            leaveSummaryByTeamList[0].LeaveSummaryInTeam.Add(leavesumary);
                        }
                        else
                        {
                            entity.LeaveSummaryInTeam.Add(leavesumary);
                        }
                    }
                }

                // When select management team, return here
                if (teams.Count == 1 && teams[0].Id == 11)
                {
                    return leaveSummaryByTeamList;
                }

                leaveSummaryByTeamList.Add(entity);
            }

            return leaveSummaryByTeamList;
        }
        #endregion
    }
}
