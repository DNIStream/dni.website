using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DNI.Services.Podcast;
using DNI.Services.Vodcast;

using Microsoft.Extensions.Logging;

namespace DNI.Services.ShowList {
    public class ShowListService : IShowListService {
        private readonly IPodcastService _podcastService;
        private readonly IVodcastService _vodcastService;
        private readonly ILogger<ShowListService> _logger;

        public ShowListService(IPodcastService podcastService, IVodcastService vodcastService, ILogger<ShowListService> logger) {
            _podcastService = podcastService;
            _vodcastService = vodcastService;
            _logger = logger;
        }

        /// <summary>
        ///     Retrieves aggregated vodcast and podcast shows, ordered in descending published version order.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Show>> GetShowsAsync() {
            // Run both data operations in parallel
            PodcastStream podcastShows = null;
            VodcastStream vodcastShows = null;

            var dataTasks = new Task[] {
                Task.Run(async () => podcastShows = await _podcastService.GetAllAsync()),
                Task.Run(async () => vodcastShows = await _vodcastService.GetAllAsync())
            };

            await Task.WhenAll(dataTasks);

            // TODO: Cache both sources until next week, but only if they return data

            // Iterate shows from both sources, matching and merging where possible (full outer join)
            // Key matching works on the version strings in the following properties:
            //      Fireside / podcast: show.url = https://podcast.dnistream.live/v9-0
            //      YouTube / vodcast: show.title = Documentation Not Included: Episode v9.0 - Shifting Security

            IEnumerable<Show> shows = null;

            if(vodcastShows?.Shows == null && podcastShows?.Shows != null) {
                // No vodcasts, return podcast info only
                shows = podcastShows.Shows
                    .Select(p => {
                        return new Show {
                            Title = p.Title,
                            Summary = p.Summary,
                            AudioUrl = p.AudioFile?.Url,
                            VideoUrl = null,
                            PublishedTime = p.DatePublished,
                            Version = p.Version,
                            ImageUrl = null,
                            ShowNotes = p.Content,
                            PodcastPageUrl = p.PageUrl,
                            Duration = p.AudioFile?.Duration,
                            VodPageUrl = null
                        };
                    });
            }

            if(podcastShows?.Shows == null && vodcastShows?.Shows != null) {
                // No podcasts, return vodcast info only
                shows = vodcastShows.Shows
                    .Select(v => new Show {
                        Title = v.Title.Replace("Documentation Not Included: ", "").Trim(),
                        Summary = null,
                        AudioUrl = null,
                        VideoUrl = v.VideoUrl,
                        PublishedTime = v.DatePublished,
                        Version = v.Version,
                        ImageUrl = v.ImageUrl,
                        ShowNotes = v.Description,
                        PodcastPageUrl = null,
                        Duration = null,
                        VodPageUrl = v.VideoPageUrl
                    });
            }

            if(podcastShows?.Shows != null && vodcastShows?.Shows != null) {
                // At least one vodcast and one podcast exists, merge
                shows = vodcastShows.Shows
                    .FullOuterJoin(podcastShows?.Shows, v => v.Version, p => p.Version,
                        (v, p, key) => {
                            return new Show {
                                Title = p?.Title ?? v?.Title.Replace("Documentation Not Included: ", "").Trim(),
                                Summary = p?.Summary ?? v?.Description,
                                AudioUrl = p?.AudioFile?.Url,
                                VideoUrl = v?.VideoUrl,
                                PublishedTime = p?.DatePublished ?? v?.DatePublished,
                                Version = key,
                                ImageUrl = v?.ImageUrl,
                                ShowNotes = v?.Description ?? p?.Content,
                                PodcastPageUrl = p?.PageUrl,
                                Duration = p?.AudioFile?.Duration,
                                VodPageUrl = v?.VideoPageUrl
                            };
                        });
            }

            return shows?
                .Where(x => !string.IsNullOrWhiteSpace(x.Version)) // Omit results with invalid version strings
                .OrderByDescending(x => x.Version);
        }

        /// <summary>
        ///     Retrieves aggregated vodcast and podcast shows, ordered by the specified field and order.
        /// </summary>
        /// <param name="orderByField"></param>
        /// <param name="orderByOrder"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Show>> GetShowsAsync(ShowOrderField orderByField, ShowOrderFieldOrder orderByOrder) {
            var shows = await GetShowsAsync();

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

            return shows;
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