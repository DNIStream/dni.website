﻿using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

using DNI.Options;

using Microsoft.Extensions.Options;

namespace DNI.Services.Email {
    /// <summary>
    ///     Wrapper for <see cref="System.Net.Mail.SmtpClient" /> for testing.
    /// </summary>
    public class SmtpClient : System.Net.Mail.SmtpClient, ISmtpClient {
        public SmtpClient(IOptions<GeneralOptions> generalOptions) {
            var o = generalOptions.Value;

            if(!string.IsNullOrWhiteSpace(o.SmtpServer)) {
                Host = o.SmtpServer;
            }

            if(o.SmtpServerPort > 0) {
                Port = o.SmtpServerPort;
            }

            EnableSsl = o.SmtpEnableSSL;

            if(!string.IsNullOrWhiteSpace(o.SmtpUsername) && !string.IsNullOrWhiteSpace(o.SmtpPassword)) {
                Credentials = new NetworkCredential(o.SmtpUsername, o.SmtpPassword);
            }
        }

        public new void Send(MailMessage message) {
            base.Send(message);
            ServicePoint.CloseConnectionGroup(ServicePoint.ConnectionName);
        }

        public new async Task SendMailAsync(MailMessage message) {
            await base.SendMailAsync(message);
            ServicePoint.CloseConnectionGroup(ServicePoint.ConnectionName);
        }
    }
}