using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DNI.Services.Podcast;
using DNI.Services.Shared.Mapping;
using DNI.Services.Shared.Paging;
using DNI.Services.Shared.Sorting;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DNI.Services.Show {
    public class ShowService : IShowService {
        private readonly IPodcastService _podcastService;
        private readonly ILogger<ShowService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IShowKeywordAggregationService _showKeywordAggregationService;
        private readonly IPagingCalculator<PodcastShow> _showPagingCalculator;
        private readonly IMapper<PodcastShow, Show> _podcastShowMapper;
        private readonly ISorter<PodcastShow> _showSorter;

        public ShowService(IPodcastService podcastService, IShowKeywordAggregationService showKeywordAggregationService,
            IPagingCalculator<PodcastShow> showPagingCalculator, IMapper<PodcastShow, Show> podcastShowMapper, ISorter<PodcastShow> showSorter,
            ILogger<ShowService> logger, IMemoryCache memoryCache) {
            _podcastService = podcastService;
            _logger = logger;
            _memoryCache = memoryCache;
            _showKeywordAggregationService = showKeywordAggregationService;
            _showPagingCalculator = showPagingCalculator;
            _podcastShowMapper = podcastShowMapper;
            _showSorter = showSorter;
        }

        private PodcastStream _podcastStream;

        private async Task<PodcastStream> GetShowsFromRssFeed() {
            if(_podcastStream == null) {
                var podcastRssGraph = await _podcastService.GetAllAsync();
                _podcastStream = podcastRssGraph;
            }

            return _podcastStream;
        }

        /// <summary>
        ///     Retrieves all shows, ordered in descending published version order.
        /// </summary>
        /// <returns></returns>
        public async Task<IPagedResponse<Show>> GetShowsAsync(IPagingRequest pagingRequest, ISortingRequest sortingRequest, string keyword = null) {
            var podcastRssGraph = await GetShowsFromRssFeed();

            if(podcastRssGraph?.Shows == null || podcastRssGraph.Shows.Count <= 0) {
                return null;
            }

            // Filter by keyword
            var shows = !string.IsNullOrWhiteSpace(keyword) ?
                podcastRssGraph.Shows.Where(x => x.Keywords.Contains(keyword)) : podcastRssGraph.Shows;

            // Perform sorting
            var orderedShows = await _showSorter.SortAsync(shows, sortingRequest);

            // Perform paging
            var pagedPodcastShows = await _showPagingCalculator.PageItemsAsync<PodcastShowPagedResponse>(orderedShows, pagingRequest);

            var mappedShows = pagedPodcastShows.Items
                .Select(x => _podcastShowMapper.Map(x));

            return new ShowsPagedResponse {
                CurrentPage = pagedPodcastShows.CurrentPage,
                EndIndex = pagedPodcastShows.EndIndex,
                StartIndex = pagedPodcastShows.StartIndex,
                TotalPages = pagedPodcastShows.TotalPages,
                TotalRecords = pagedPodcastShows.TotalRecords,
                ItemsPerPage = pagedPodcastShows.ItemsPerPage,
                Items = mappedShows
            };
        }

        /// <summary>
        ///     Retrieves the latest show
        /// </summary>
        /// <returns></returns>
        public async Task<Show> GetLatestShowAsync() {
            var podcastRssGraph = await GetShowsFromRssFeed();

            var firstShow = podcastRssGraph.Shows
                .OrderByDescending(x => x.PublishedTime)
                .First();

            return _podcastShowMapper.Map(firstShow);
        }

        /// <summary>
        ///     Retrieves a distinct list of keywords from the shows returned by the podcast service, along with a count of the
        ///     number of shows they are present in
        /// </summary>
        /// <returns></returns>
        public async Task<IDictionary<string, int>> GetAggregatedKeywords() {
            var podcastRssGraph = await GetShowsFromRssFeed();
            var keywordCounts = await _showKeywordAggregationService.GetKeywordDictionaryAsync(podcastRssGraph.Shows);
            return keywordCounts;
        }

        /// <summary>
        ///     Retrieves a podcast show by its slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<Show> GetShowBySlugAsync(string slug) {
            if(string.IsNullOrWhiteSpace(slug)) {
                throw new ArgumentNullException(nameof(slug));
            }

            var allPodcasts = await _podcastService.GetAllAsync();

            var podcastShow = allPodcasts.Shows.FirstOrDefault(x => x.Slug == slug);
            if(podcastShow == null) {
                throw new InvalidOperationException($"Slug '{slug}' not found");
            }

            var mappedShow = _podcastShowMapper.Map(podcastShow);

            return mappedShow;
        }
    }
}