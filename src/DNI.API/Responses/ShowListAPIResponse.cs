using System.Collections.Generic;

using DNI.Services.Show;

namespace DNI.API.Responses {
    /// <summary>
    ///     A response that contains a paged list of shows, plus the latest show, and an aggregated count of all keywords for
    ///     all shows.
    /// </summary>
    public class ShowListAPIResponse {
        /// <summary>
        ///     The paging information for the <see cref="PagedShows" /> enumeration
        /// </summary>
        public PagedAPIResponse PageInfo { get; set; }

        /// <summary>
        ///     The shows for this pages
        /// </summary>
        public IEnumerable<Show> PagedShows { get; set; }

        /// <summary>
        ///     The latest show - always the same regardless of the paging and ordering queries
        /// </summary>
        public Show LatestShow { get; set; }

        /// <summary>
        ///     An aggregated list of keywords across all un-paged shows
        /// </summary>
        public IDictionary<string, int> ShowKeywords { get; set; }
    }
}