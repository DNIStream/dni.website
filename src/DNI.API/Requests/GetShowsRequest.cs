using DNI.Services.Shared.Paging;
using DNI.Services.Shared.Sorting;

using Microsoft.AspNetCore.Mvc;

namespace DNI.API.Requests {
    /// <summary>
    ///     A REST request for retrieving a list of shows
    /// </summary>
    public class GetShowsRequest : IPagingInfo, ISortingInfo {
        /// <summary>
        ///     The page number to request
        /// </summary>
        [BindProperty(Name = "page-num", SupportsGet = true)]
        public int PageNumber { get; set; } = PagingConstants.DEFAULT_PAGE_INDEX + 1;

        /// <summary>
        ///     The number of items to return per page
        /// </summary>
        [BindProperty(Name = "items-per-page", SupportsGet = true)]
        public int ItemsPerPage { get; set; } = PagingConstants.DEFAULT_PAGE_SIZE;

        /// <summary>
        ///     The field to sort by
        /// </summary>
        [BindProperty(Name = "sort-by-fields", SupportsGet = true)]
        public FieldSort SortByField { get; set; }
    }
}