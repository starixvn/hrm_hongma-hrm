namespace Fanex.NexHR.Core.Repositories
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface ILookupDataRepository
    {
        IList<LookupData> GetAll(string lookupCode);
    }
}
