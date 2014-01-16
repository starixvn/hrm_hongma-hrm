// --------------------------------------------------------------------------
// <copyright file="ILeaveTypeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface ILeaveTypeService
    {
        IList<LeaveType> GetAll();

        LeaveType GetLeaveType(int id);
    }
}
