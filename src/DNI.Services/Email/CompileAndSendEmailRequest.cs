using System.Collections.Generic;
using System.Reflection;

namespace DNI.Services.Email {
    public class CompileAndSendEmailRequest {
        /// <summary>
        ///     Required. The address that the email will be sent to
        /// </summary>
        public string To { get; set; }

        /// <summary>
        ///     Required. The address that the email will be sent from
        /// </summary>
        public string From { get; set; }

        /// <summary>
        ///     The address where email replies will be sent.
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        ///     Required. The assembly that the email resources are located in. Both layout and templates should be located in the
        ///     same assembly.
        /// </summary>
        public Assembly ResourceAssembly { get; set; }

        /// <summary>
        ///     Required. The resource identifier for the email template to use
        /// </summary>
        public string EmailTemplateResourceId { get; set; }

        /// <summary>
        ///     Required. The resource identifier for the HTML layout template
        /// </summary>
        public string HtmlLayoutResourceId { get; set; }

        /// <summary>
        ///     The placeholder replacement key / value pairs that will be used to customise the template values in the email.
        /// </summary>
        public IDictionary<string, string> TemplateReplacements { get; set; }

        /// <summary>
        ///     The placeholder replacement key / value pairs that will be used to customise the layout values in the email.
        /// </summary>
        public IDictionary<string, string> LayoutReplacements { get; set; }
    }
}