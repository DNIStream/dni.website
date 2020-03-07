namespace DNI.API.Responses {
    /// <summary>
    ///     A response containing paging information
    /// </summary>
    public class PagedAPIResponse {
        /// <summary>
        ///     The page number that is currently being returned
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        ///     The total number of records for all pages
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        ///     The total pages available
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        ///     The zero based start index of the record for the returned page
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        ///     The zero based end index of the record for the returned page
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        ///     The number of items per page
        /// </summary>
        public int ItemsPerPage { get; set; }
    }
}