using System.Collections.Generic;

using DNI.Services.Shared.Paging;

namespace DNI.Services.Show {
    /// <summary>
    ///     Contains a list of shows along with aggregated metadata and paging information.
    /// </summary>
    public class ShowsPagedResponse: IPagedResponse<Show> {

        public int CurrentPage { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public int ItemsPerPage { get; set; }

        public IEnumerable<Show> Items { get; set; }
    }
}