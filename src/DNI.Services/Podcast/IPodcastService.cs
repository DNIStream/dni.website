using System.Threading.Tasks;

namespace DNI.Services.Podcast {
    /// <summary>
    ///     Provides an interface for creating a service that returns podcast show data
    /// </summary>
    public interface IPodcastService {
        Task<PodcastStream> GetAllAsync();
    }
}