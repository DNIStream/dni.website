using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNI.Services.ShowList {
    public interface IShowListService {
        Task<IEnumerable<Show>> GetShowsAsync();
    }
}