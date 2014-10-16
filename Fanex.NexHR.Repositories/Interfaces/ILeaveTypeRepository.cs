// --------------------------------------------------------------------------
// <copyright file="ILeaveTypeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface ILeaveTypeRepository
    {
        IList<LeaveType> GetAll(bool excludeNotShown = true);

        LeaveType GetLeaveType(int id);
    }
}
