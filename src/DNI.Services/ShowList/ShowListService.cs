using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DNI.Services.Podcast;

using Microsoft.Extensions.Logging;

namespace DNI.Services.ShowList {
    public class ShowListService : IShowListService {
        private readonly IPodcastService _podcastService;
        private readonly ILogger<ShowListService> _logger;

        public ShowListService(IPodcastService podcastService, ILogger<ShowListService> logger) {
            _podcastService = podcastService;
            _logger = logger;
        }

        /// <summary>
        ///     Retrieves all shows, ordered in descending published version order.
        /// </summary>
        /// <returns></returns>
        public async Task<ShowList> GetShowListAsync() {
            var podcastShows = await _podcastService.GetAllAsync();

            // TODO: Caching
            // TODO: Paging

            var shows = podcastShows.Shows
                .Select(p => new Show {
                    Title = p.Title,
                    Summary = p.Summary,
                    AudioUrl = p.AudioFile?.Url,
                    PublishedTime = p.DatePublished,
                    Version = p.Version,
                    ImageUrl = p.HeaderImage,
                    ShowNotes = p.Content,
                    ShowNotesHtml = p.ContentHtml,
                    PodcastPageUrl = p.PageUrl,
                    Duration = p.AudioFile?.Duration,
                    Slug = p.Slug,
                    Keywords = p.Keywords
                })
                .ToArray();

            var keywordCounts = shows
                .SelectMany(x => x.Keywords)
                .GroupBy(k => k, (keyword, keywords) => new {Keyword = keyword, Count = keywords.Count()})
                .ToDictionary(k => k.Keyword, v => v.Count);

            return new ShowList {
                Shows = shows.OrderByDescending(x => x.Version),
                TotalKeywordCounts = keywordCounts
            };
        }

        /// <summary>
        ///     Retrieves shows, ordered by the specified field and order.
        /// </summary>
        /// <param name="orderByField"></param>
        /// <param name="orderByOrder"></param>
        /// <returns></returns>
        public async Task<ShowList> GetShowListAsync(ShowOrderField orderByField, ShowOrderFieldOrder orderByOrder) {
            var shows = (await GetShowListAsync()).Shows;

            // No need to use a reflection based / dynamic implementation as there are
            // currently only two fields to order by. When new fields are added, this
            // switch should not need expanding - update the OrderShow() method instead.
            switch(orderByOrder) {
                case ShowOrderFieldOrder.Ascending:
                    shows = shows.OrderBy(s => OrderShow(s, orderByField));
                    break;
                case ShowOrderFieldOrder.Descending:
                    shows = shows.OrderByDescending(s => OrderShow(s, orderByField));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderByOrder), orderByOrder, "Specified order not supported");
            }

            return new ShowList {
                Shows = shows
            };
        }

        /// <summary>
        ///     Basic field selection method for ordering. This could get cumbersome if more fields are introduced, so may be
        ///     refactored as necessary in the future.
        /// </summary>
        /// <param name="show"></param>
        /// <param name="orderByField"></param>
        /// <returns></returns>
        private static object OrderShow(Show show, ShowOrderField orderByField) {
            switch(orderByField) {
                case ShowOrderField.PublishedTime:
                    return show.PublishedTime;
                case ShowOrderField.Version:
                    return show.Version;
                default:
                    return null;
            }
        }
    }
}