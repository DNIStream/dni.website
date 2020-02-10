using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNI.Services.Shared.Paging {
    /// <summary>
    ///     Provides paging information from API calls and queries.
    /// </summary>
    public class PagingCalculator<TItem> : IPagingCalculator<TItem>
        where TItem : class {
        /// <summary>
        ///     Calculates paging information for the specified <see cref="IEnumerable{T}" /> and returns a paged result set.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TPagedResponse"></typeparam>
        /// <param name="pagingInfo"></param>
        /// <param name="allItems"></param>
        /// <returns></returns>
        public async Task<TPagedResponse> PageItemsAsync<TPagedResponse>(IPagingInfo pagingInfo, TItem[] allItems)
            where TPagedResponse : IPagedResponse<TItem>, new() {
            if(pagingInfo == null) {
                throw new ArgumentNullException(nameof(pagingInfo));
            }

            if(pagingInfo.ItemsPerPage <= 0) {
                throw new InvalidOperationException("pagingInfo.ItemsPerPage must be greater than zero");
            }

            if(pagingInfo.PageNumber <= 0) {
                throw new InvalidOperationException("pagingInfo.PageNumber must be greater than zero");
            }

            var pageSize = pagingInfo.ItemsPerPage;
            var totalRecords = allItems.Length;
            var totalPages = CalculateTotalPages(totalRecords, pageSize);
            var currentPageNumber = CalculateCurrentPageNumber(totalPages, pagingInfo.PageNumber);
            var startIndex = CalculateStartIndex(totalRecords, currentPageNumber, pageSize, totalPages);
            var endIndex = CalculateEndIndex(totalRecords, currentPageNumber, pageSize, totalPages, startIndex);

            return new TPagedResponse {
                CurrentPage = currentPageNumber,
                TotalRecords = totalRecords,
                StartIndex = startIndex,
                // Check for divide by zero error
                TotalPages = totalPages,
                EndIndex = endIndex,
                // TODO: Benchmark
                Items = await FilterItemsAsync(allItems, startIndex, pageSize)
            };
        }

        /// <summary>
        ///     Performs paging on the input items
        /// </summary>
        /// <param name="allItems"></param>
        /// <param name="startIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private async Task<TItem[]> FilterItemsAsync(TItem[] allItems, int startIndex, int pageSize) {
            return (await Task.Run(() => allItems.Skip(startIndex).Take(pageSize))).ToArray();
        }

        /// <summary>
        ///     Calculates the current page number
        /// </summary>
        /// <param name="totalPages"></param>
        /// <param name="requestedPageNumber"></param>
        /// <returns></returns>
        private int CalculateCurrentPageNumber(int totalPages, int requestedPageNumber) {
            if(totalPages == 0) {
                return -1;
            }

            if(requestedPageNumber > totalPages) {
                return totalPages;
            }

            return requestedPageNumber;
        }

        /// <summary>
        ///     Zero based index of the first record returned.
        /// </summary>
        private int CalculateStartIndex(int totalRecords, int currentPageNumber, int pageSize, int totalPages) {
            if(totalRecords == 0) {
                return -1;
            }

            if(currentPageNumber > totalPages) {
                return -1;
            }

            var startIndex = (currentPageNumber * pageSize) - pageSize;
            return startIndex;
        }

        /// <summary>
        ///     Zero based index of the last record returned
        /// </summary>
        private int CalculateEndIndex(int totalRecords, int currentPageNumber, int pageSize, int totalPages, int startIndex) {
            if(totalRecords == 0) {
                return -1;
            }

            if(totalRecords == 0) {
                return 0;
            }

            if(currentPageNumber > totalPages) {
                return -1;
            }

            var endIndex = startIndex + (pageSize - 1);
            if((endIndex + 1) > totalRecords) {
                endIndex = totalRecords - 1;
            }

            return endIndex;
        }

        /// <summary>
        ///     Calculates the total number of pages rounded up whole number.
        /// </summary>
        /// <returns></returns>
        private int CalculateTotalPages(decimal totalRecords, int pageSize) {
            return (int) Math.Ceiling(totalRecords / pageSize);
        }
    }
}