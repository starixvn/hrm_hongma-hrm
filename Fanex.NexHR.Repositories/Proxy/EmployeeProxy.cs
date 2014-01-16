// --------------------------------------------------------------------------
// <copyright file="EmployeeProxy.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Proxy
{    
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    public class EmployeeProxy : Employee
    {
        private readonly ITeamRepository _groupRepository = new TeamRepository();

        public int GroupId;

        public override Team Team
        {
            get
            {
                if (string.IsNullOrEmpty(base.Team.Name))
                {
                    base.Team = _groupRepository.Get(GroupId);
                }
                return base.Team;
            }
            set
            {
                base.Team = value;
            }
        }
    }
}
