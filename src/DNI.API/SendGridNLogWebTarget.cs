using System;
using System.ComponentModel;
using System.Threading.Tasks;

using DNI.Services.Email;

using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace DNI.API {
    /// <summary>
    ///     Custom NLog target for sending emails via SendGrid
    /// </summary>
    [Target("SendGrid")]
    public class SendGridNLogWebTarget : TargetWithLayout {
        public SendGridNLogWebTarget() {
            // TODO: Inject EmailService and Environment
        }

        /// <summary>
        ///     The email address to send the email to
        /// </summary>
        [RequiredParameter]
        public Layout To { get; set; }

        /// <summary>
        ///     The email address to send the email from
        /// </summary>
        [RequiredParameter]
        public Layout From { get; set; }

        /// <summary>
        ///     The body of the email. Same as Layout.
        /// </summary>
        [RequiredParameter]
        [DefaultValue("${message}${newline}")]
        public Layout Body {
            get => Layout;
            set => Layout = value;
        }

        /// <summary>
        ///     The subject of the email to be sent. Supports Layouts.
        /// </summary>
        [RequiredParameter]
        public Layout Subject { get; set; }

        protected IEmailService EmailService { get; set; }

        protected string Environment { get; set; }

        protected override void Write(LogEventInfo logEvent) {
            InternalLogger.Info($"SendGrid: Sending email message in '{Environment}' configuration");

            var to = To.Render(logEvent);
            Task.WaitAll(EmailService.SendAsync(new SendEmailRequest {
                To = to,
                From = From.Render(logEvent),
                Body = Body.Render(logEvent),
                Subject = Subject.Render(logEvent)
            }));

            InternalLogger.Info($"SendGrid: Email message sent using '{Environment}' configuration");
        }
    }
}