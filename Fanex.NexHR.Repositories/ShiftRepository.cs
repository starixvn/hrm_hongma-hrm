// --------------------------------------------------------------------------
// <copyright file="ShiftRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Shift Repository.
    /// </summary>
    public class ShiftRepository : IShiftRepository
    {
        public IList<Shift> GetAll()
        {
            using (var db = new ObjectDb("Shift_GetShifts"))
            {
                return db.Query<Shift>().ToList();
            }
        }

        public Shift GetByDate(int employeeId, DateTime date)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetShiftByDate"))
            {
                return db.Query<Shift>(new { employeeId, date }).FirstOrDefault();
            }
        }
    }
}
