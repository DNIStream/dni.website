using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DNI.Services.Email {
    /// <summary>
    ///     Sends email via System.Net.Mail.
    /// </summary>
    public class SystemNetEmailService : EmailTemplateServiceBase {
        private readonly ISmtpClient _smtpClient;

        /// <summary>
        ///     Instantiates the System.Net.Mail email service
        /// </summary>
        public SystemNetEmailService(ISmtpClient smtpClient) {
            _smtpClient = smtpClient;
        }

        /// <summary>
        ///     Sends an email message asynchronously
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public override async Task<bool> SendAsync(SendEmailRequest req) {
            ValidateRequest(req);

            var msg = CompileMessage(req);

            await _smtpClient.SendMailAsync(msg);

            return true;
        }

        private static MailMessage CompileMessage(SendEmailRequest req) {
            var msg = new MailMessage {
                To = {
                    req.To
                },
                Subject = req.Subject,
                IsBodyHtml = !string.IsNullOrEmpty(req.HtmlBody),
                ReplyToList = {
                    new MailAddress(req.ReplyTo)
                }
            };
            if(!string.IsNullOrWhiteSpace(req.From)) {
                msg.From = new MailAddress(req.From);
            }

            // Add plain text body
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(req.Body, null, MediaTypeNames.Text.Plain));

            if(!string.IsNullOrWhiteSpace(req.HtmlBody)) {
                // Add HTML body
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(req.HtmlBody, null, MediaTypeNames.Text.Html));
            }

            return msg;
        }
    }
}