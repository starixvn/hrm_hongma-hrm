// --------------------------------------------------------------------------
// <copyright file="ILookupDataRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface ILookupDataRepository
    {
        IList<LookupData> GetAll();

        IList<LookupData> GetAll(string lookupCode);

        IList<LookupData> GetJobTitle(string jobTitle);

        bool Delete(int id);

        bool Insert(string name, string lookupCode);

        bool Update(int id, string name);
    }
}
