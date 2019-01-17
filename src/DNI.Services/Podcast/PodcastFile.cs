using RestSharp.Deserializers;

namespace DNI.Services.Podcast {
    public class PodcastFile {
        [DeserializeAs(Name = "url")]
        public string Url { get; set; }

        [DeserializeAs(Name = "mime_type")]
        public string MimeType { get; set; }

        [DeserializeAs(Name = "size_in_bytes")]
        public long SizeInBytes { get; set; }

        [DeserializeAs(Name = "duration_in_seconds")]
        public long DurationSeconds { get; set; }
    }
}