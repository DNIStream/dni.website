using System.Collections.Generic;

namespace DNI.Services.Shared.Paging {
    public interface IPagedResponse<TItem> {
        int CurrentPage { get; set; }

        int TotalRecords { get; set; }

        int TotalPages { get; set; }

        int StartIndex { get; set; }

        int EndIndex { get; set; }

        int ItemsPerPage { get; set; }

        IEnumerable<TItem> Items { get; set; }
    }
}