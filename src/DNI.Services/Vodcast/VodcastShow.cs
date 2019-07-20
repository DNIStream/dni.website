using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using RestSharp.Deserializers;

using static System.Decimal;
using static System.String;

namespace DNI.Services.Vodcast {
    /// <summary>
    ///     Represents a single Vodcast
    /// </summary>
    public class VodcastShow {
        private readonly Regex vodcastTitleMatcher = new Regex(@"Documentation Not Included: Episode v(\d+\.\d+)( ?)-{1}(.+)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        [DeserializeAs(Name = "id")]
        public string Id { get; set; }

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
        public string Version {
            get {
                var m = vodcastTitleMatcher.Match(Title);
                if(!m.Success) {
                    return null;
                }

                var version = m.Groups[1].Value.Trim();
                if(TryParse(version, out _)) {
                    return version;
                }

                return null;
            }
        }

        /// <summary>
        ///     Returns the YouTube embed video url
        /// </summary>
        [IgnoreDataMember]
        public string VideoUrl => IsNullOrWhiteSpace(Id) ? null : Concat("https://www.youtube.com/embed/", Id);

        /// <summary>
        ///     Returns the YouTube video watch url
        /// </summary>
        [IgnoreDataMember]
        public string VideoPageUrl => IsNullOrWhiteSpace(Id) ? null : Concat("https://www.youtube.com/watch?v=", Id);
    }
}