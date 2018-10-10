using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;

namespace DNI.Services.Email {
    /// <summary>
    ///     Sends emails via SendGrid.
    /// </summary>
    public class SendGridEmailService : EmailTemplateServiceBase {
        private readonly ISendGridClient _smtpClient;

        public SendGridEmailService(ISendGridClient smtpClient) {
            _smtpClient = smtpClient;
        }

        /// <summary>
        ///     Asynchronously sends an email message
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public override async Task<bool> SendAsync(SendEmailRequest req) {
            ValidateRequest(req);

            var msg = CompileMessage(req);

            await _smtpClient.SendEmailAsync(msg);

            return true;
        }

        private static SendGridMessage CompileMessage(SendEmailRequest req) {
            var msg = new SendGridMessage {
                From = new EmailAddress(req.From),
                Subject = req.Subject,
                PlainTextContent = req.Body,
                HtmlContent = req.HtmlBody,
                ReplyTo = new EmailAddress(req.ReplyTo)
            };
            msg.AddTo(req.To);
            return msg;
        }
    }
}