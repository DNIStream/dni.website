using System.Linq;
using System.Threading.Tasks;

using DNI.Services.Podcast;
using DNI.Services.Shared.Mapping;
using DNI.Services.Shared.Paging;
using DNI.Services.Shared.Sorting;

using Microsoft.Extensions.Logging;

namespace DNI.Services.ShowList {
    public class ShowListService : IShowListService {
        private readonly IPodcastService _podcastService;
        private readonly ILogger<ShowListService> _logger;
        private readonly IShowKeywordAggregationService _showKeywordAggregationService;
        private readonly IPagingCalculator<Show> _showPagingCalculator;
        private readonly IMapper<PodcastShow, Show> _podcastShowMapper;
        private readonly ISorter<Show> _showSorter;

        public ShowListService(IPodcastService podcastService, IShowKeywordAggregationService showKeywordAggregationService,
            IPagingCalculator<Show> showPagingCalculator, IMapper<PodcastShow, Show> podcastShowMapper, ISorter<Show> showSorter, ILogger<ShowListService> logger) {
            _podcastService = podcastService;
            _logger = logger;
            _showKeywordAggregationService = showKeywordAggregationService;
            _showPagingCalculator = showPagingCalculator;
            _podcastShowMapper = podcastShowMapper;
            _showSorter = showSorter;
        }

        /// <summary>
        ///     Retrieves all shows, ordered in descending published version order.
        /// </summary>
        /// <returns></returns>
        public async Task<ShowList> GetShowListAsync(IPagingInfo pageInfo, ISortingInfo sortingInfo) {
            var podcastShows = await _podcastService.GetAllAsync();

            // Domain object mapping
            var allShows = podcastShows.Shows
                .Select(x => _podcastShowMapper.Map(x))
                .ToArray();

            // Keyword aggregation
            var keywordCounts = await _showKeywordAggregationService.GetKeywordDictionaryAsync(allShows);

            // TODO: Caching

            // Perform sorting
            var orderedShows = await _showSorter.SortAsync(allShows, sortingInfo);

            // Perform paging
            var showListResponse = await _showPagingCalculator.PageItemsAsync<ShowList>(pageInfo, orderedShows);
            showListResponse.TotalKeywordCounts = keywordCounts;
            return showListResponse;
        }
    }
}