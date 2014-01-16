// --------------------------------------------------------------------------
// <copyright file="LeaveTypeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Leave type repository.
    /// </summary>
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        public IList<LeaveType> GetAll(bool excludeNotShown = true)
        {
            using (var db = new ObjectDb("LeaveType_GetAll"))
            {
                return db.Query<LeaveType>(new { excludeNotShown }).ToList();
            }
        }

        public LeaveType GetLeaveType(int id)
        {
            using (var db = new ObjectDb("LeaveType_GetById"))
            {
                return db.Query<LeaveType>(new { id }).FirstOrDefault();
            }
        }
    }
}
