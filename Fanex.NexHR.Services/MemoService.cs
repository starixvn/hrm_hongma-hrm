// --------------------------------------------------------------------------
// <copyright file="MemoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Memo Service.
    /// </summary>
    public class MemoService : IMemoService
    {
        private readonly IMemoRepository _memoRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public MemoService(IMemoRepository memoRepository = null, IEmployeeRepository employeeRepository = null, IEmailService emailService = null)
        {
            this._memoRepository = memoRepository ?? new MemoRepository();
            this._employeeRepository = employeeRepository ?? new EmployeeRepository();
            this._emailService = emailService ?? new EmailService();
        }

        public IList<Memo> GetAll(MemoViewModel model)
        {
            IList<Memo> entityList = null;
            entityList = this._memoRepository.GetAll(model.Content, model.Year, model.Category);

            return entityList;
        }

        /// <summary>
        /// Get memo by id.
        /// </summary>
        /// <param name="id">The id of memo.</param>
        /// <returns>Memo object.</returns>
        public Memo GetById(int id)
        {
            Memo memo = new Memo();
            if (id > 0)
            {
                memo = this._memoRepository.GetById(id);
                if (memo != null && memo.Type == 1)
                {
                    memo.AttachFile = memo.Contents;
                    memo.Contents = "<a href='" + HttpUtility.HtmlEncode(memo.AttachFile) + "'> " 
                        + HttpUtility.HtmlEncode(memo.AttachFile.Split('/')[memo.AttachFile.Split('/').Length - 1]) + "</a>";
                }
            }

            return memo;
        }

        public bool Delete(int id)
        {
            bool result = false;
            if (id > 0)
            {
                result = this._memoRepository.Delete(id);
            }

            return result;
        }

        /// <summary>
        /// Function to insert MEMO.
        /// </summary>
        /// <param name="entity">MEMO Object.</param>
        /// <returns>Result of insert.</returns>
        public int Insert(Memo entity)
        {
            int result = 1;
            if (entity != null)
            {
                result = this._memoRepository.Insert(entity);
                var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;
                if (result != -1 && entity.SendMail)
                {
                    var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
                    var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
                    //// send email(s)
                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/MemoNotification.cshtml"));

                    var content = new
                    {
                        Subject = entity.Subject,
                        Link = link + "memo/detail/" + result.ToString()
                    };
                    var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        groupGo,
                        string.Empty,
                        Resources.Messages.MemoSubject + " " + entity.Subject,
                        body);
                    this._emailService.SendEmail(
                       ConfigurationManager.AppSettings["Email"],
                       groupSd,
                       string.Empty,
                       Resources.Messages.MemoSubject + " " + entity.Subject,
                       body);
                }
            }

            return result;
        }

        /// <summary>
        /// Function to update MEMO.
        /// </summary>
        /// <param name="entity">MEMO Object.</param>
        /// <returns>Result of updated.</returns>
        public bool Update(Memo entity)
        {
            bool result = false;
            if (entity != null)
            {
                result = this._memoRepository.Update(entity);
                var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;

                if (result && entity.SendMail)
                {
                    var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
                    var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
                    //// send email(s)
                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/EditMemoNotification.cshtml"));

                    var content = new
                    {
                        Subject = entity.Subject,
                        Link = link + "memo/detail/" + entity.Id.ToString()
                    };
                    var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        groupGo,
                        string.Empty,
                        Resources.Messages.MemoSubject + " " + entity.Subject,
                        body);
                    this._emailService.SendEmail(
                       ConfigurationManager.AppSettings["Email"],
                       groupSd,
                       string.Empty,
                       Resources.Messages.MemoSubject + " " + entity.Subject,
                       body);
                }
            }

            return result;
        }
    }
}
