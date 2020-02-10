using System.Threading.Tasks;

using DNI.Services.Shared.Paging;
using DNI.Services.Shared.Sorting;

namespace DNI.Services.ShowList {
    public interface IShowListService {
        Task<ShowList> GetShowListAsync(IPagingInfo pageInfo, ISortingInfo sortingInfo);
    }
}