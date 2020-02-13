using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DNI.Services.Podcast;

namespace DNI.Services.Show {
    public class ShowKeywordAggregationService : IShowKeywordAggregationService {
        /// <summary>
        ///     Aggregates all keywords from <see cref="shows" /> and returns the results in a dictionary
        /// </summary>
        /// <param name="shows"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, int>> GetKeywordDictionaryAsync(IEnumerable<PodcastShow> shows) {
            return await Task.FromResult(shows
                .SelectMany(x => x.Keywords)
                .GroupBy(k => k, (keyword, keywords) => new {Keyword = keyword, Count = keywords.Count()})
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Keyword)
                .ToDictionary(k => k.Keyword, v => v.Count));
        }
    }
}