using System.Threading.Tasks;

namespace DNI.Services.Shared.Paging {
    /// <summary>
    ///     Provides an interface for a generic enumeration paging calculator
    /// </summary>
    public interface IPagingCalculator<TItem>
        where TItem : class {
        Task<TPagedResponse> PageItemsAsync<TPagedResponse>(TItem[] allItems, IPagingRequest pagingRequest)
            where TPagedResponse : IPagedResponse<TItem>, new();
    }
}