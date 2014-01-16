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
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NexHR.Models;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
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
