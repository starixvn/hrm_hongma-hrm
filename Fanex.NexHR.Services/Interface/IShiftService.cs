// --------------------------------------------------------------------------
// <copyright file="IShiftService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;

    public interface IShiftService
    {
        IList<Shift> GetAll();

        IList<SelectListItem> GetSelectList();

        Shift GetByDate(int employeeId, DateTime date);
    }
}
