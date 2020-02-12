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
        /// <param name="sortingInfo"></param>
        /// <returns></returns>
        public async Task<TItem[]> SortAsync(IEnumerable<TItem> allItems, ISortingInfo sortingInfo) {
            // Validate input
            if(sortingInfo == null) {
                throw new ArgumentNullException(nameof(sortingInfo));
            }

            if(string.IsNullOrWhiteSpace(sortingInfo.Field)) {
                throw new ArgumentException("sortingInfo.Field is required", nameof(sortingInfo));
            }

            if(allItems == null) {
                throw new ArgumentNullException(nameof(allItems));
            }

            return await Task.Run(() => {
                if(sortingInfo.Order == FieldOrder.Ascending) {
                    return allItems.OrderBy(x => OrderByGenericPropertyAsync(x, sortingInfo)).ToArray();
                }

                return allItems.OrderByDescending(x => OrderByGenericPropertyAsync(x, sortingInfo)).ToArray();
            });
        }

        // Perform sorting
        private static object OrderByGenericPropertyAsync(TItem item, ISortingInfo sortingInfo) {
            var type = typeof(TItem);
            var propInfo = type.GetProperty(sortingInfo.Field);
            if(propInfo == null) {
                throw new InvalidOperationException($"'{sortingInfo.Field}' is not a property of type '{type.Name}'");
            }

            return propInfo.GetValue(item);
        }
    }
}