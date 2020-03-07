using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNI.Services.Shared.Sorting {
    /// <summary>
    ///     A service that provides generic <see cref="IEnumerable{T}" /> sorting
    /// </summary>
    public class Sorter<TItem> : ISorter<TItem>
        where TItem : class {
        /// <summary>
        ///     Sorts the specified <see cref="IEnumerable{TItem}" />
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="allItems"></param>
        /// <param name="sortingRequest"></param>
        /// <returns></returns>
        public async Task<TItem[]> SortAsync(IEnumerable<TItem> allItems, ISortingRequest sortingRequest) {
            // Validate input
            if(sortingRequest == null) {
                throw new ArgumentNullException(nameof(sortingRequest));
            }

            if(string.IsNullOrWhiteSpace(sortingRequest.Field)) {
                throw new ArgumentException("sortingRequest.Field is required", nameof(sortingRequest));
            }

            if(allItems == null) {
                throw new ArgumentNullException(nameof(allItems));
            }

            return await Task.Run(() => {
                if(sortingRequest.Order == FieldOrder.Ascending) {
                    return allItems.OrderBy(x => OrderByGenericPropertyAsync(x, sortingRequest)).ToArray();
                }

                return allItems.OrderByDescending(x => OrderByGenericPropertyAsync(x, sortingRequest)).ToArray();
            });
        }

        // Perform sorting
        private static object OrderByGenericPropertyAsync(TItem item, ISortingRequest sortingRequest) {
            var type = typeof(TItem);
            var propInfo = type.GetProperty(sortingRequest.Field);
            if(propInfo == null) {
                throw new SortingException($"'{sortingRequest.Field}' is not a property of type '{type.Name}'");
            }

            return propInfo.GetValue(item);
        }
    }
}