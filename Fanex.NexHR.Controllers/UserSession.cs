using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Controllers
{
    [Serializable]
    public class UserSession : IUserSession
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public short Role { get; set; }
    }
}
