using System.Threading.Tasks;

namespace DNI.Services.Vodcast {
    /// <summary>
    ///     Provides an interface for creating a service that returns Vodcast show data
    /// </summary>
    public interface IVodcastService {
        Task<VodcastStream> GetAllAsync();
    }
}