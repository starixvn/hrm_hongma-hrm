// --------------------------------------------------------------------------
// <copyright file="LookupDataRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
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
    /// Lookup Data Repository.
    /// </summary>
    public class LookupDataRepository : ILookupDataRepository
    {
        public IList<LookupData> GetAll(string lookupCode)
        {
            using (var db = new ObjectDb("Lookup_GetLookupData"))
            {
                return db.Query<LookupData>(new { lookupCode }).ToList();
            }
        }

        public IList<LookupData> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<LookupData> GetJobTitle(string jobTitle)
        {
            using (var db = new ObjectDb("Lookup_GetJobTitle"))
            {
                return db.Query<LookupData>(new { jobTitle }).ToList();
            }
        }

        public bool Delete(int id)
        {
            using (ObjectDb db = new ObjectDb("LookupData_Delete"))
            {
                return db.ExecuteNonQuery(new { id }) != 0;
            }
        }

        public bool Insert(string name, string lookupCode)
        {
            using (ObjectDb db = new ObjectDb("LookupData_Insert"))
            {
                return db.ExecuteNonQuery(new { name, lookupCode }) != 0;
            }
        }

        public bool Update(int id, string name)
        {
            using (ObjectDb db = new ObjectDb("LookupData_Update"))
            {
                return db.ExecuteNonQuery(new { id, name }) != 0;
            }
        }
    }
}
