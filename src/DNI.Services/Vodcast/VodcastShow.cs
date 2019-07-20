using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using RestSharp.Deserializers;

namespace DNI.Services.Vodcast {
    public class VodcastShow {

        private readonly Regex vodcastTitleMatcher = new Regex(@"Documentation Not Included: Episode v(\d+\.\d+)( ?)-{1}(.+)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        [DeserializeAs(Name = "id")] public string Id { get; set; }

        [DeserializeAs(Name = "snippet.title")]
        public string Title { get; set; }

        [DeserializeAs(Name = "snippet.resourceId.videoId")]
        public string VideoId { get; set; }

        [DeserializeAs(Name = "snippet.description")]
        public string Description { get; set; }

        [DeserializeAs(Name = "contentDetails.videoPublishedAt")]
        public DateTime DatePublished { get; set; }

        [DeserializeAs(Name = "snippet.thumbnails.high.url")]
        public string ImageUrl { get; set; }

        [IgnoreDataMember]
        public decimal? Version {
            get {
                var m = vodcastTitleMatcher.Match(Title);
                if(!m.Success) {
                    return null;
                }

                var version = m.Groups[1].Value.Trim();
                if(decimal.TryParse(version, out var dVersion)) {
                    return dVersion;
                }

                return null;
            }
        }
    }
}