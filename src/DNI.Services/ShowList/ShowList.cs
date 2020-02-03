using System.Collections.Generic;

namespace DNI.Services.ShowList {
    /// <summary>
    ///     Contains a list of shows along with aggregated metadata and paging information.
    /// </summary>
    public class ShowList {
        public IEnumerable<Show> Shows { get; set; }
    }
}