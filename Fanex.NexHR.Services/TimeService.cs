// --------------------------------------------------------------------------
// <copyright file="TimeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.6.20">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.Repositories.Interface;
    using Fanex.NexHR.Repositories;

    public class TimeService : ITimeService
    {
        private readonly ITimeRepository _timeRepository;

        public TimeService(ITimeRepository timeRepository = null)
        {
            this._timeRepository = timeRepository ?? new TimeRepository();
        }

        public bool ImportTimeRecord()
        {
            return this._timeRepository.ImportTimeRecord();
        }
    }
}
