// --------------------------------------------------------------------------
// <copyright file="TeamService.cs" company="Nexcel Solutions Vietnam">
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
    using System.Linq;
    using Fanex.Caching;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Team Service.
    /// </summary>
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private ICache _cacheManager;

        public TeamService(ITeamRepository teamRespository = null)
        {
            this._teamRepository = teamRespository ?? new TeamRepository();
            this._cacheManager = CacheService.CreateInstance();
        }

        public Team Get(int teamId)
        {
            Team team = new Team();
            if (teamId > 0)
            {
                var allTeams = this.GetAll();
                team = allTeams.Where(t => t.Id == teamId).SingleOrDefault();
            }

            return team;
        }

        public Team Get(string code)
        {
            Team team = new Team();
            if (string.IsNullOrWhiteSpace(code))
            {
                var allTeams = this.GetAll();
                team = allTeams.Single(t => t.Code == code);
            }

            return team;
        }

        public Team GetGroupOfEmployee(int employeeId)
        {
            Team team = new Team();

            if (employeeId > 0)
            {
                team = this._teamRepository.GetGroupOfEmployee(employeeId);
            }

            return team;
        }

        public IList<Team> GetAll()
        {
            return this._teamRepository.GetAll();
        }

        public IList<Team> GetTeamManagedByEmp(int managerId)
        {
            return this._teamRepository.GetTeamManagedByEmp(managerId);
        }

        public IList<Team> GetSubTeamsOfEmployee(int employeeId)
        {
            return this._teamRepository.GetSubTeamsOfEmployee(employeeId);
        }

        public int AddNewTeam(TeamViewModel model)
        {
            return this._teamRepository.AddNewTeam(model);
        }

        public bool UpdateTeam(TeamViewModel model)
        {
            return this._teamRepository.UpdateTeam(model);
        }

        public bool DeleteTeam(int id, DateTime modifiedDate, int modifiedby, out int errCode, out string message)
        {
            return this._teamRepository.DeleteTeam(id, modifiedDate, modifiedby, out errCode, out message);
        }

        public IList<Team> GetTeamsHandleRequestsOfEmployee(int employeeId)
        {
            return this._teamRepository.GetTeamsHandleRequestsOfEmployee(employeeId);
        }
    }
}