namespace Fanex.NexHR.Core.Repositories
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IGroupRepository
    {
        Team Get(int id);

        Team Get(string code);

        Team GetGroupOfEmployee(int employeeId);

        IList<Team> GetAll();
    }
}
