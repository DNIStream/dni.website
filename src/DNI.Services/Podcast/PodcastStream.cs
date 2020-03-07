using System.Collections.Generic;

namespace DNI.Services.Podcast {
    public class PodcastStream {
        public IList<PodcastShow> Shows { get; set; } = new List<PodcastShow>();
    }
}