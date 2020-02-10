﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNI.Services.Shared.Sorting {
    /// <summary>
    ///     Provides a sorting service contract.
    /// </summary>
    public interface ISorter {
        Task<TItem[]> SortAsync<TItem>(IEnumerable<TItem> allItems, ISortingInfo sortingInfo);
    }
}