// --------------------------------------------------------------------------
// <copyright file="IMemoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using Fanex.NexHR.ViewModels;

    public interface IMemoService
    {
        bool Delete(int id);

        System.Collections.Generic.IList<Fanex.NexHR.Models.Memo> GetAll(MemoViewModel model);

        Fanex.NexHR.Models.Memo GetById(int id);

        int Insert(Fanex.NexHR.Models.Memo entity);

        bool Update(Fanex.NexHR.Models.Memo entity);
    }
}
