using System;
using System.Text.RegularExpressions;

namespace DNI.Services.Podcast {
    /// <summary>
    ///     Represents a single podcast
    /// </summary>
    public class PodcastShow {
        private readonly Regex podcastUriVersionMatcher =
            new Regex(@"/v(\d+-\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string PageUrl { get; set; }

        public string Content { get; set; }

        public string ContentHtml { get; set; }

        public string Summary { get; set; }

        public DateTime DatePublished { get; set; }

        public PodcastFile AudioFile { get; set; }

        public string Version {
            get {
                var m = podcastUriVersionMatcher.Match(PageUrl);
                if(!m.Success) {
                    return null;
                }

                var version = m.Groups[1].Value.Replace("-", ".").Trim();

                if(decimal.TryParse(version, out _)) {
                    return version;
                }

                return null;
            }
        }
    }
}