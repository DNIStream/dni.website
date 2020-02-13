using System.Collections.Generic;
using System.Threading.Tasks;

using DNI.Services.Podcast;

namespace DNI.Services.Show {
    public interface IShowKeywordAggregationService {
        /// <summary>
        ///     Returns a dictionary of unique keywords along with a count of all instances in the show list
        /// </summary>
        /// <param name="shows"></param>
        /// <returns></returns>
        Task<IDictionary<string, int>> GetKeywordDictionaryAsync(IEnumerable<PodcastShow> shows);
    }
}