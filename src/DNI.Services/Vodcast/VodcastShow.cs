using System;

using RestSharp.Deserializers;

namespace DNI.Services.Vodcast {
    public class VodcastShow {
        [DeserializeAs(Name = "id")] public Guid Id { get; set; }

        [DeserializeAs(Name = "title")] public string Title { get; set; }

        [DeserializeAs(Name = "url")] public string Url { get; set; }

        [DeserializeAs(Name = "content_text")] public string Content { get; set; }

        [DeserializeAs(Name = "content_html")] public string ContentHtml { get; set; }

        [DeserializeAs(Name = "summary")] public string Summary { get; set; }

        [DeserializeAs(Name = "date_published")]
        public DateTime DatePublished { get; set; }
    }
}