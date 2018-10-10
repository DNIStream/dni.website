#pragma warning disable IDE1006
namespace DNI.Services.Email {
    public class EmailTemplate {
        /// <summary>
        ///     The email address the email will be sent from
        /// </summary>
        public string from { get; set; }

        /// <summary>
        ///     The email subject
        /// </summary>
        public string subject { get; set; }

        /// <summary>
        ///     Text that will be inserted into the HTML template, but only shown on clients that support preview text
        /// </summary>
        public string preHeaderHtml { get; set; }

        /// <summary>
        ///     Will be inserted into the HTML template before the call to action button
        /// </summary>
        public string introHtml { get; set; }

        // Naming Styles
        /// <summary>
        ///     Will be inserted into the HTML template after the call to action button, but before the signature
        /// </summary>
        public string outroHtml { get; set; }

        /// <summary>
        ///     The link for the call to action button
        /// </summary>
        public string actionLink { get; set; }

        /// <summary>
        ///     The text for the call to action button
        /// </summary>
        public string actionText { get; set; }

        /// <summary>
        ///     The plain text version of the email.
        /// </summary>
        public string plainTextBody { get; set; }
    }
}