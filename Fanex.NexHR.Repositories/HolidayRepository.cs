// --------------------------------------------------------------------------
// <copyright file="HolidayRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Holiday Repository.
    /// </summary>
    public class HolidayRepository : IHolidayRepository
    {
        /// <summary>
        /// Method insert Holiday.
        /// </summary>
        /// <param name="model">Holiday model.</param>
        /// <returns>Inserted holiday's id.</returns>
        public int Insert(Holiday model)
        {
            using (var db = new ObjectDb("Holiday_Insert"))
            {
                db.AddInParameter("@HolidayName", DbType.String, model.HolidayName);
                db.AddInParameter("@FromDate", DbType.DateTime, model.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, model.ToDate);
                db.AddInParameter("@ModifiedBy", DbType.Int16, model.ModifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);

                return (int)db.ExecuteScalar();
            }
        }

        public bool Delete(int id, int modifiedBy)
        {
            using (var db = new ObjectDb("Holiday_Delete"))
            {
                db.AddInParameter("@Id", DbType.Int32, id);
                db.AddInParameter("@ModifiedBy", DbType.Int16, modifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Method update Holiday.
        /// </summary>
        /// <param name="model">Holiday model.</param>
        /// <returns>Id of updated holiday.</returns>
        public bool Update(Holiday model)
        {
            using (ObjectDb db = new ObjectDb("Holiday_Update"))
            {
                db.AddInParameter("@Id", DbType.Int32, model.Id);
                db.AddInParameter("@HolidayName", DbType.String, model.HolidayName);
                db.AddInParameter("@FromDate", DbType.DateTime, model.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, model.ToDate);
                db.AddInParameter("@ModifiedBy", DbType.Int16, model.ModifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);

                return db.ExecuteNonQuery() != 0;
            }
        }

        public IList<Holiday> GetByYear(int year)
        {
            using (var db = new ObjectDb("Holiday_GetByYear"))
            {
                return db.Query<Holiday>(new { year }).ToList();
            }
        }

        public Holiday GetById(int id)
        {
            using (var db = new ObjectDb("Holiday_GetById"))
            {
                return db.QueryEntity<Holiday>(new { id });
            }
        }
    }
}
