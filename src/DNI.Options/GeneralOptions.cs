namespace DNI.Options {
    /// <summary>
    ///     A class that holds general / shared configuration options
    /// </summary>
    public class GeneralOptions {
        public string Env { get; set; }

        public string Version { get; set; }

        public string ErrorEmailFrom { get; set; }

        public string ErrorEmailTo { get; set; }

        public string WebBaseUri { get; set; }

        public string ContactEmailTo { get; set; }

        public string HtmlEmailLayoutResourceId { get; set; }

        public string ContactTemplateResourceId { get; set; }

        public string LogoPath { get; set; }

        public string SmtpServer { get; set; }

        public int SmtpServerPort { get; set; }

        public bool SmtpEnableSSL { get; set; }

        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }

        public string PodcastServiceBaseUri { get; set; }

        public string VodcastServiceBaseUri { get; set; }

        public string PodcastServiceResourceUri { get; set; }
    }
}