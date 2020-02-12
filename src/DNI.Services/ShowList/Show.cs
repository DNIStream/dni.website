using System;
using System.Collections.Generic;

namespace DNI.Services.ShowList {
    public class Show {
        public string AudioUrl { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string ShowNotes { get; set; }

        public string ShowNotesHtml { get; set; }

        public DateTime PublishedTime { get; set; }

        public decimal? Version { get; set; }

        public string Slug { get; set; }

        public string ImageUrl { get; set; }

        public string PodcastPageUrl { get; set; }

        public string Duration { get; set; }

        public long? DurationInSeconds { get; set; }

        public IEnumerable<string> Keywords { get; set; }
    }
}