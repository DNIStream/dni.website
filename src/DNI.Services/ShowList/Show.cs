using System;

namespace DNI.Services.ShowList {
    public class Show {
        public string VideoUrl { get; set; }

        public string AudioUrl { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string ShowNotes { get; set; }

        public DateTime? PublishedTime { get; set; }

        public string Version { get; set; }

        public string ImageUrl { get; set; }

        public string VodPageUrl { get; set; }

        public string PodcastPageUrl { get; set; }

        public string Duration { get; set; }
    }
}