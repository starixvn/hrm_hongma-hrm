// --------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Fanex.NexHR.Models;

    public interface IRepository<TId, TEntity> where TEntity : IEntity<TId>
    {        
        TEntity Add(TEntity entity);

        bool Update(TEntity entity);

        bool Delete(TId id);

        TEntity Single(TId id);

        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        TEntity First(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetQuery();

        IEnumerable<TEntity> GetAll();

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}
