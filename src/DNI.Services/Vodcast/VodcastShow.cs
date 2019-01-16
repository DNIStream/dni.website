using System;

using RestSharp.Deserializers;

namespace DNI.Services.Vodcast {
    public class VodcastShow {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }

        [DeserializeAs(Name = "snippet.title")]
        public string Title { get; set; }

        [DeserializeAs(Name = "snippet.resourceId.videoId")]
        public string VideoId { get; set; }

        [DeserializeAs(Name = "snippet.description")]
        public string Content { get; set; }

        [DeserializeAs(Name = "snippet.thumbnails.maxres.url")]
        public string Thumbnail { get; set; }

        [DeserializeAs(Name = "snippet.publishedAt")]
        public DateTime DatePublished { get; set; }

        [DeserializeAs(Name = "snippet.thumbnails.high.url")]
        public string ImageUrl { get; set; }
    }
}