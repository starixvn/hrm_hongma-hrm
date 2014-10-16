// --------------------------------------------------------------------------
// <copyright file="ITeamRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface ITeamRepository
    {
        Team Get(int id);

        Team Get(string code);

        Team GetGroupOfEmployee(int employeeId);

        IList<Team> GetAll();

        IList<Team> GetTeamManagedByEmp(int managerId);

        IList<Team> GetSubTeamsOfEmployee(int employeeId);

        int AddNewTeam(TeamViewModel model);

        bool UpdateTeam(TeamViewModel model);

        bool DeleteTeam(int id, DateTime modifiedDate, int modifiedBy, out int errorCode, out string message);

        IList<Team> GetTeamsHandleRequestsOfEmployee(int employeeId);
    }
}
