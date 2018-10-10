namespace DNI.Services.Email {
    public class SendEmailRequest {
        /// <summary>
        ///     The address that this message will be sent to
        /// </summary>
        public string To { get; set; }

        /// <summary>
        ///     The address that this message will be sent from
        /// </summary>
        public string From { get; set; }

        /// <summary>
        ///     Reply Address
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        ///     The plain text body of the email.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     The HTML body of the email.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        ///     The subject of the email
        /// </summary>
        public string Subject { get; set; }
    }
}