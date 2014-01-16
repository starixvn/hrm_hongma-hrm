// --------------------------------------------------------------------------
// <copyright file="MemoRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Memo Repository.
    /// </summary>
    public class MemoRepository : IMemoRepository
    {
        public IList<Memo> GetAll(string content, int year, int category)
        {
            using (var db = new ObjectDb("Memo_GetAll"))
            {
                return db.Query<Memo>(new { content = content, year = year, category = category }).ToList();
            }
        }

        public Memo GetById(int id)
        {
            using (var db = new ObjectDb("Memo_GetById"))
            {
                return db.QueryEntity<Memo>(new { id });
            }
        }

        /// <summary>
        /// Method delete a memo.
        /// </summary>
        /// <param name="id">Id of memo.</param>
        /// <returns>Boolean to check status.</returns>
        public bool Delete(int id)
        {
            using (var db = new ObjectDb("Memo_Delete"))
            {
                db.AddInParameter("@Id", DbType.Int32, id);
                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Method insert a memo.
        /// </summary>
        /// <param name="model">Memo model.</param>
        /// <returns>Id of new memo.</returns>
        public int Insert(Memo model)
        {
            using (var db = new ObjectDb("Memo_Insert"))
            {
                db.AddInParameter("@RefNo", DbType.String, model.RefNo);
                db.AddInParameter("@Contents", DbType.String, model.Contents);
                db.AddInParameter("@Subject", DbType.String, model.Subject);
                db.AddInParameter("@SentTo", DbType.String, model.SentTo);
                db.AddInParameter("@SentFrom", DbType.String, model.SentFrom);
                db.AddInParameter("@ForInfo", DbType.String, model.ForInfo);
                db.AddInParameter("@TotalPages", DbType.Int32, model.TotalPages);
                db.AddInParameter("@SentDate", DbType.DateTime, model.SentDate);
                db.AddInParameter("@Category", DbType.Int32, model.Category);
                db.AddInParameter("@Type", DbType.Int32, model.Type);

                return (int)db.ExecuteScalar();
            }
        }

        /// <summary>
        /// Method update a Memo.
        /// </summary>
        /// <param name="model">Memo model.</param>
        /// <returns>Boolean to check status.</returns>
        public bool Update(Memo model)
        {
            using (var db = new ObjectDb("Memo_Update"))
            {
                db.AddInParameter("@Id", DbType.Int32, model.Id);
                db.AddInParameter("@RefNo", DbType.String, model.RefNo);
                db.AddInParameter("@Contents", DbType.String, model.Contents);
                db.AddInParameter("@Subject", DbType.String, model.Subject);
                db.AddInParameter("@SentTo", DbType.String, model.SentTo);
                db.AddInParameter("@SentFrom", DbType.String, model.SentFrom);
                db.AddInParameter("@ForInfo", DbType.String, model.ForInfo);
                db.AddInParameter("@TotalPages", DbType.Int32, model.TotalPages);
                db.AddInParameter("@SentDate", DbType.DateTime, model.SentDate);
                db.AddInParameter("@Category", DbType.Int32, model.Category);
                db.AddInParameter("@Type", DbType.Int32, model.Type);

                return db.ExecuteNonQuery() != 0;
            }
        }
    }
}
