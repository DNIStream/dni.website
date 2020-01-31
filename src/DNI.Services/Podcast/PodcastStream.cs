using System.Collections.Generic;

using RestSharp.Deserializers;

namespace DNI.Services.Podcast {
    public class PodcastStream {
        [DeserializeAs(Name = "items")]
        public List<PodcastShow> Shows { get; set; } = new List<PodcastShow>();
    }
}