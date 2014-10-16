// --------------------------------------------------------------------------
// <copyright file="Employee.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Attributes;

    /// <summary>
    /// Employee Class.
    /// </summary>
    public class Employee : EntityBase<int>
    {
        public string DomainUsername { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string Password { get; set; }

        public string EmployeeCard { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Email]
        public string Email { get; set; }

        public int? JobTitleId { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public string Phone { get; set; }

        public byte? Gender { get; set; }

        public Team Team { get; set; }

        public string Roles { get; set; }

        public Employee Manager { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ConfirmedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ResignedDate { get; set; }

        public string Skype { get; set; }

        public string Yahoo { get; set; }

        public bool Mariage { get; set; }

        public DateTime? LastLogin { get; set; }

        public decimal AnnualLeaveBalance { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee Creator { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Employee Modifier { get; set; }

        [Display(Name = "Other team(s)")]
        public string SubGroups { get; set; }

        [Display(Name = "Teams' leaders receive requests")]
        public string LeaderGroups { get; set; }

        public string JobTitle { get; set; }

        public string Address { get; set; }

        public string CurrentAddress { get; set; }

        public string CardNo { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        public IList<short> RoleList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return null;
                }

                var roles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var result = new List<short>();

                foreach (var item in roles)
                {
                    result.Add(short.Parse(item));
                }

                return result;
            }

            set
            {
            }
        }

        public string[] RoleArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return null;
                }

                return Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set
            {
                Roles = Utilities.StringArrayToString(value);
            }
        }

        public string[] SubGroupArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SubGroups))
                {
                    return null;
                }

                return SubGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set
            {
                SubGroups = Utilities.StringArrayToString(value);
            }
        }

        public string[] LeaderGroupsArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LeaderGroups))
                {
                    return null;
                }

                return LeaderGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set
            {
                LeaderGroups = Utilities.StringArrayToString(value);
            }
        }

        public string DisplayName
        {
            get
            {
                return (string.IsNullOrEmpty(NickName) || string.IsNullOrEmpty(FullName))
                ? string.Empty
                : NickName + " (" + FullName + ")";
            }
        }

        /// <summary>
        /// Check user is in specific roles.
        /// </summary>
        /// <param name="roles">Role of user.</param>
        /// <returns>Return Boolean.</returns>
        public bool IsInRoles(params RolesEnum[] roles)
        {
            if (RoleList == null)
            {
                return false;
            }

            var userRoles = System.Web.Security.Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role.EnumToString()));
        }

        /// <summary>
        /// Check user is in specific roles.
        /// </summary>
        /// <param name="roles">Role of user.</param>
        /// <returns>Return Boolean.</returns>
        public bool IsInRoles(IList<string> roles)
        {
            if (roles == null || roles.Count == 0)
            {
                return false;
            }

            var userRoles = System.Web.Security.Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role));
        }

        /// <summary>
        /// Display role user.
        /// </summary>
        /// <returns>Role text.</returns>
        public string DisplayRoles()
        {
            var roles = "Member";
            if (string.IsNullOrWhiteSpace(Roles))
            {
                return roles;
            }

            foreach (var item in RoleArray)
            {
                switch (item)
                {
                    case "1":
                        roles += ", AssistantLeader";
                        break;
                    case "2":
                        roles += ", Leader";
                        break;
                    case "3":
                        roles += ", HR";
                        break;
                    case "4":
                        roles = "System admin";
                        break;
                }
            }

            return roles;
        }
    }
}
