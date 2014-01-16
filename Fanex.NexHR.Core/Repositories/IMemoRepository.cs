namespace Fanex.NexHR.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Models;

    public interface IMemoRepository
    {
        IList<Memo> GetAll();

        Memo GetById(int id);

        int Insert(Memo model);

        bool Delete(int id);

        bool Update(Memo model);
    }
}
