// --------------------------------------------------------------------------
// <copyright file="EmailService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Configuration;
    using Fanex.MailDeliver.Model;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Email Service.
    /// </summary>
    public class EmailService : IEmailService
    {
        public void SendEmail(string senderEmail, string receiverEmail, string mailCc, string subject, string body)
        {
            this.SendEmail(senderEmail, senderEmail, receiverEmail, mailCc, subject, body);
        }

        /// <summary>
        /// Function to send mail.
        /// </summary>
        /// <param name="senderEmail">Sender Email.</param>
        /// <param name="senderName">Sender Name.</param>
        /// <param name="receiverEmail">Receiver Email.</param>
        /// <param name="mailCc">The Mail CC.</param>
        /// <param name="subject">Subject email.</param>
        /// <param name="body">Body of email.</param>
        public void SendEmail(string senderEmail, string senderName, string receiverEmail, string mailCc,  string subject, string body)
        {
            if (!Utilities.IsValidEmail(senderEmail) || !Utilities.IsValidEmail(receiverEmail))
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = receiverEmail,
                MailCc = mailCc,
                Subject = subject,
                Title = ConfigurationManager.AppSettings["Email.SenderName"],
                Body = body
            };
            using (var client = new Fanex.MailDeliver.MailClient.API.MailDeliver())
            {
                client.SendMail(mailItem);
            }
        }
    }
}
