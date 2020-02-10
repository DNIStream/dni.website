using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNI.Services.Shared.Sorting {
    /// <summary>
    ///     A service that provides generic <see cref="IEnumerable{T}" /> sorting
    /// </summary>
    public class Sorter : ISorter {
        /// <summary>
        ///     Sorts the specified <see cref="IEnumerable{TItem}" />
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="allItems"></param>
        /// <param name="sortingInfo"></param>
        /// <returns></returns>
        public async Task<TItem[]> SortAsync<TItem>(IEnumerable<TItem> allItems, ISortingInfo sortingInfo) {
            //// No need to use a reflection based / dynamic implementation as there are
            //// currently only two fields to order by. When new fields are added, this
            //// switch should not need expanding - update the OrderShow() method instead.
            //switch(sortingRequest.Order) {
            //    case FieldOrder.Ascending:
            //        shows = shows.OrderBy(s => OrderShow(s, sortingRequest.Field));
            //        break;
            //    case FieldOrder.Descending:
            //        shows = shows.OrderByDescending(s => OrderShow(s, sortingRequest.Field));
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException(nameof(sortingRequest.Order), sortingRequest.Order, "Specified order not supported");
            //}

            throw new NotImplementedException();
        }

        ///// <summary>
        /////     Basic field selection method for ordering. This could get cumbersome if more fields are introduced, so may be
        /////     refactored as necessary in the future.
        ///// </summary>
        ///// <param name="show"></param>
        ///// <param name="orderByField"></param>
        ///// <returns></returns>
        //private static object OrderShow(Show show, string orderByField) {
        //    switch(orderByField) {
        //        case "PublishedTime":
        //            return show.PublishedTime;
        //        case "Version":
        //            return show.Version;
        //        default:
        //            return null;
        //    }
        //}
    }
}