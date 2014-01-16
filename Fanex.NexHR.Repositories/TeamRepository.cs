// --------------------------------------------------------------------------
// <copyright file="TeamRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Team Repository.
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        /// <summary>
        /// Get Team by id.
        /// </summary>
        /// <param name="id">Team's id.</param>
        /// <returns>Team entity.</returns>
        public Team Get(int id)
        {
            using (var db = new ObjectDb("GetGroup"))
            {
                var team = new Team();

                using (IDataReader reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            team = new Team
                            {
                                Id = reader.ToInt("Id"),
                                Code = reader.ToString("Code"),
                                Name = reader.ToString("Name"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                                Modifer = new Employee
                                {
                                    Id = reader.To<int>("ModiedBy")
                                },

                                Leader = new Employee
                                {
                                    NickName = reader.ToString("LeaderNickName"),
                                    FullName = reader.ToString("LeaderFullName"),
                                    Id = reader.ToInt("LeaderId"),
                                },
                                AssistantLeader = new Employee
                                {
                                    NickName = reader.ToString("AssistantLeaderNickName"),
                                    FullName = reader.ToString("AssistantLeaderFullName"),
                                    Id = reader.ToInt("AssistantLeaderId")
                                }
                            };
                        }
                    }
                }

                return team;
            }
        }

        public Team Get(string code)
        {
            using (var db = new ObjectDb("GetGroup"))
            {
                var entity = db.Query<Team>(new { code }).FirstOrDefault();
                return entity;
            }
        }

        public Team GetGroupOfEmployee(int employeeId)
        {
            using (var db = new ObjectDb("GetGroupOfEmployee"))
            {
                var entity = db.Query<Team>(new { employeeId }).FirstOrDefault();
                return entity;
            }
        }

        /// <summary>
        /// Get all Team.
        /// </summary>
        /// <returns>List of Team.</returns>
        public IList<Team> GetAll()
        {
            using (var db = new ObjectDb("GetGroups"))
            {
                IList<Team> entityList = new List<Team>();

                using (IDataReader reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new Team
                            {
                                Id = reader.ToInt("Id"),
                                Code = reader.ToString("Code"),
                                Name = reader.ToString("Name"),
                                Leader = new Employee
                                {
                                    NickName = reader.ToString("LeaderNickName"),
                                    FullName = reader.ToString("LeaderFullName"),
                                    Id = reader.ToInt("LeaderId"),
                                },
                                AssistantLeader = new Employee
                                {
                                    NickName = reader.To<string>("AssistantLeaderNickName"),
                                    FullName = reader.To<string>("AssistantLeaderFullName"),
                                    Id = reader.To<int>("AssistantLeaderId")
                                }
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        /// <summary>
        /// Method update Team.
        /// </summary>
        /// <param name="model">Team ViewModel.</param>
        /// <returns>Boolean to check status.</returns>
        public bool UpdateTeam(TeamViewModel model)
        {
            using (var db = new ObjectDb("EmployeeGroup_Update"))
            {
                db.AddInParameter("@Id", DbType.Int32, model.Id);
                db.AddInParameter("@Code", DbType.String, model.TeamCode);
                db.AddInParameter("@Name", DbType.String, model.TeamName);
                db.AddInParameter("@LeaderId", DbType.Int32, model.LeaderId);
                db.AddInParameter("@AssistantLeaderId", DbType.Int32, model.AssistantLeaderId);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, model.ModifiedBy);

                try
                {
                    return db.ExecuteNonQuery() != 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        public IList<Team> GetTeamManagedByEmp(int managerId)
        {
            using (var db = new ObjectDb("GetTeamManagedById"))
            {
                var entityList = db.Query<Team>(new { managerId }).ToList();
                return entityList;
            }
        }

        public IList<Team> GetSubTeamsOfEmployee(int employeeId)
        {
            using (var db = new ObjectDb("Employee_GetSubTeam"))
            {
                var entityList = db.Query<Team>(new { empId = employeeId }).ToList();
                return entityList;
            }
        }

        /// <summary>
        /// Method Add new Team.
        /// </summary>
        /// <param name="model">Team ViewModel.</param>
        /// <returns>Id of Team.</returns>
        public int AddNewTeam(TeamViewModel model)
        {
            using (var db = new ObjectDb("EmployeeGroup_Insert"))
            {
                db.AddInParameter("@Code", DbType.String, model.TeamCode);
                db.AddInParameter("@Name", DbType.String, model.TeamName);
                db.AddInParameter("@LeaderId", DbType.Int32, model.LeaderId);
                db.AddInParameter("@AssistantLeaderId", DbType.Int32, model.AssistantLeaderId);
                db.AddInParameter("@CreatedDate", DbType.DateTime, model.CreatedDate);
                db.AddInParameter("@CreatedBy", DbType.Int32, model.CreatedBy);

                return (int)db.ExecuteScalar();
            }
        }

        /// <summary>
        /// Method Delete Team.
        /// </summary>
        /// <param name="id">Id of Team.</param>
        /// <param name="modifiedDate">Modified Date.</param>
        /// <param name="modifiedBy">Modified By.</param>
        /// <param name="errorCode">Error Code.</param>
        /// <param name="message">Delete message.</param>
        /// <returns>Boolean to check status.</returns>
        public bool DeleteTeam(int id, DateTime modifiedDate, int modifiedBy, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("EmployeeGroup_Delete"))
            {
                db.AddInParameter("@Id", DbType.Int32, id);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar() == null ? false : true;

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        public IList<Team> GetTeamsHandleRequestsOfEmployee(int employeeId)
        {
            using (var db = new ObjectDb("Employee_GetGroupHandleRequest"))
            {
                var entityList = db.Query<Team>(new { id = employeeId }).ToList();
                return entityList;
            }
        }
    }
}
