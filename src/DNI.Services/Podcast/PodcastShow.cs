using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using RestSharp.Deserializers;

namespace DNI.Services.Podcast {
    public class PodcastShow {
        private readonly Regex podcastUriMatcher =
            new Regex(@"/v(\d+-\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        [DeserializeAs(Name = "id")] public Guid Id { get; set; }

        [DeserializeAs(Name = "title")] public string Title { get; set; }

        [DeserializeAs(Name = "url")] public string PageUrl { get; set; }

        [DeserializeAs(Name = "content_text")] public string Content { get; set; }

        [DeserializeAs(Name = "content_html")] public string ContentHtml { get; set; }

        [DeserializeAs(Name = "summary")] public string Summary { get; set; }

        [DeserializeAs(Name = "date_published")]
        public DateTime DatePublished { get; set; }

        [DeserializeAs(Name = "attachments")] public IEnumerable<PodcastFile> Files { get; set; }

        [IgnoreDataMember]
        public decimal? Version {
            get {
                var m = podcastUriMatcher.Match(PageUrl);
                if(!m.Success) {
                    return null;
                }

                var version = m.Groups[1].Value.Replace("-", ".").Trim();

                if(decimal.TryParse(version, out var dVersion)) {
                    return dVersion;
                }

                return null;
            }
        }
    }
}