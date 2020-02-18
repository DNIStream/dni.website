using System.Collections.Generic;

using DNI.Services.Podcast;
using DNI.Services.Shared.Paging;

namespace DNI.Services.Show {
    public class PodcastShowPagedResponse : IPagedResponse<PodcastShow> {
        public int CurrentPage { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public int ItemsPerPage { get; set; }

        public IEnumerable<PodcastShow> Items { get; set; }
    }
}