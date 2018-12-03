using System;

namespace DNI.Services.ShowList {
    public class Show {
        public string VideoUrl { get; set; }

        public string AudioUrl { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public DateTime PublishedTime { get; set; }
    }
}