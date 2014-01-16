// --------------------------------------------------------------------------
// <copyright file="TimeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.6.20">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.NexHR.Repositories.Interface;

    /// <summary>
    /// Time Repository.
    /// </summary>
    public class TimeRepository : ITimeRepository
    {
        /// <summary>
        /// Method Import Time Record.
        /// </summary>
        /// <returns>Boolean value to check.</returns>
        public bool ImportTimeRecord()
        {
            using (var db = new ObjectDb("TimeRecord_ImportData"))
            {
                try
                {
                    return db.ExecuteNonQuery() != 0;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
