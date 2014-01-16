// --------------------------------------------------------------------------
// <copyright file="IEmailService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    public interface IEmailService
    {
        void SendEmail(string senderEmail, string receiverEmail, string mailCc, string subject, string body);

        void SendEmail(string senderEmail, string senderName, string receiverEmail, string mailCc, string subject, string body);
    }
}
