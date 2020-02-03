using System.Threading.Tasks;

namespace DNI.Services.ShowList {
    public interface IShowListService {
        Task<ShowList> GetShowListAsync();

        Task<ShowList> GetShowListAsync(ShowOrderField orderByField, ShowOrderFieldOrder orderByOrder);
    }
}