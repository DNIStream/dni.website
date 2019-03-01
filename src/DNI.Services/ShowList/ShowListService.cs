using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DNI.Services.Podcast;
using DNI.Services.Vodcast;

using Microsoft.Extensions.Logging;

namespace DNI.Services.ShowList {
    public class ShowListService : IShowListService {
        private readonly IPodcastService _podcastService;
        private readonly IVodcastService _vodcastService;
        private readonly ILogger<ShowListService> _logger;

        private readonly Regex podcastUriMatcher = new Regex(@"/v(\d+-\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private readonly Regex vodcastTitleMatcher = new Regex(@"Documentation Not Included: Episode v(\d+\.\d+)( ?)-{1}(.+)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

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
                        var mp3File = p.Files.FirstOrDefault();

                        return new Show {
                            Title = p.Title,
                            Summary = p.Summary,
                            AudioUrl = mp3File?.Url,
                            VideoUrl = null,
                            PublishedTime = p.DatePublished,
                            Version = GetPodcastVersion(p),
                            ImageUrl = null,
                            ShowNotes = p.Content,
                            PodcastPageUrl = p.PageUrl,
                            DurationSeconds = mp3File?.DurationSeconds,
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
                        VideoUrl = GetVideoUrl(v.VideoId),
                        PublishedTime = v.DatePublished,
                        Version = GetVodcastVersion(v),
                        ImageUrl = v.ImageUrl,
                        ShowNotes = v.Description,
                        PodcastPageUrl = null,
                        DurationSeconds = null,
                        VodPageUrl = GetVideoPageUrl(v.VideoId)
                    });
            }

            if(podcastShows?.Shows != null && vodcastShows?.Shows != null) {
                // At least one vodcast and one podcast exists, merge
                shows = vodcastShows.Shows
                    .FullOuterJoin(podcastShows?.Shows, GetVodcastVersion, GetPodcastVersion,
                        (v, p, key) => {
                            var mp3File = p?.Files.FirstOrDefault();
                            return new Show {
                                Title = p?.Title ?? v?.Title.Replace("Documentation Not Included: ", "").Trim(),
                                Summary = p?.Summary ?? v?.Description,
                                AudioUrl = mp3File?.Url,
                                VideoUrl = GetVideoUrl(v?.VideoId),
                                PublishedTime = p?.DatePublished ?? v?.DatePublished,
                                Version = key,
                                ImageUrl = v?.ImageUrl,
                                ShowNotes = v?.Description ?? p?.Content,
                                PodcastPageUrl = p?.PageUrl,
                                DurationSeconds = mp3File?.DurationSeconds,
                                VodPageUrl = GetVideoPageUrl(v?.VideoId)
                            };
                        });
            }

            return shows?
                .Where(x => x.Version.HasValue) // Omit results with invalid version strings
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

        #region Helpers

        /// <summary>
        ///     Returns a YouTube embed video url
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        private static string GetVideoUrl(string videoId) {
            if(string.IsNullOrWhiteSpace(videoId)) {
                return null;
            }

            return string.Concat("https://www.youtube.com/embed/", videoId);
        }

        private static string GetVideoPageUrl(string videoId) {
            if(string.IsNullOrWhiteSpace(videoId)) {
                return null;
            }

            return string.Concat("https://www.youtube.com/watch?v=", videoId);
        }

        /// <summary>
        ///     Extracts the version of the podcast as a decimal from the url property
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        private decimal? GetPodcastVersion(PodcastShow show) {
            var m = podcastUriMatcher.Match(show.PageUrl);
            if(!m.Success) {
                return null;
            }

            var version = m.Groups[1].Value.Replace("-", ".").Trim();
            return ConvertVersion(version);
        }

        /// <summary>
        ///     Extracts the version of the vodcast as a decimal from the title property
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        private decimal? GetVodcastVersion(VodcastShow show) {
            var m = vodcastTitleMatcher.Match(show.Title);
            if(!m.Success) {
                return null;
            }

            var version = m.Groups[1].Value.Trim();
            return ConvertVersion(version);
        }

        /// <summary>
        ///     Converts a string to a nullable decimal
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static decimal? ConvertVersion(string version) {
            if(decimal.TryParse(version, out var dVersion)) {
                return dVersion;
            }

            return null;
        }

        #endregion
    }
}