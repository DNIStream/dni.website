using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DNI.Services.Podcast;
using DNI.Services.Vodcast;

namespace DNI.Services.ShowList {
    public class ShowListService : IShowListService {
        private readonly IPodcastService _podcastService;
        private readonly IVodcastService _vodcastService;

        private readonly Regex podcastUriMatcher = new Regex(@"/v(\d+-\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private readonly Regex vodcastTitleMatcher = new Regex(@"Documentation Not Included: Episode v(\d+\.\d+) -(.+)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public ShowListService(IPodcastService podcastService, IVodcastService vodcastService) {
            _podcastService = podcastService;
            _vodcastService = vodcastService;
        }

        /// <summary>
        ///     Retrieves aggregated vodcast and podcast shows, ordered in descending published version order.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Show>> GetShowsAsync() {
            var podcastShows = await _podcastService.GetAllAsync();
            var vodcastShows = await _vodcastService.GetAllAsync();

            // Iterate shows from both sources, matching and merging where possible (full outer join)
            // Key matching works on the version strings in the following properties:
            //      Fireside / podcast: show.url = https://podcast.dnistream.live/v9-0
            //      YouTube / vodcast: show.title = Documentation Not Included: Episode v9.0 - Shifting Security

            IEnumerable<Show> shows = null;

            if(vodcastShows?.Shows == null && podcastShows?.Shows != null) {
                // No vodcasts, return podcast info only
                shows = podcastShows.Shows
                    .Select(x => new Show {
                        Title = x.Title,
                        Summary = x.Summary,
                        AudioUrl = x.Url,
                        VideoUrl = null,
                        PublishedTime = x.DatePublished,
                        Version = GetPodcastVersion(x),
                        ImageUrl = null // TODO: 
                    });
            }

            if(podcastShows?.Shows == null && vodcastShows?.Shows != null) {
                // No podcasts, return vodcast info only
                shows = vodcastShows.Shows
                    .Select(x => new Show {
                        Title = x.Title,
                        Summary = x.Content,
                        AudioUrl = null,
                        VideoUrl = GetVideoUrl(x.VideoId),
                        PublishedTime = x.DatePublished,
                        Version = GetVodcastVersion(x),
                        ImageUrl = x.ImageUrl
                    });
            }

            if(podcastShows?.Shows != null && vodcastShows?.Shows != null) {
                // At least one vodcast and one podcast exists, merge
                shows = vodcastShows.Shows
                    .FullOuterJoin(podcastShows?.Shows, GetVodcastVersion, GetPodcastVersion,
                        (v, p, key) => new Show {
                            Title = p?.Title ?? v.Title,
                            Summary = p?.Summary ?? v.Content,
                            AudioUrl = p?.Url,
                            VideoUrl = GetVideoUrl(v?.VideoId),
                            PublishedTime = p?.DatePublished ?? v.DatePublished,
                            Version = key,
                            ImageUrl = v?.ImageUrl
                        });
            }

            return shows?
                .Where(x => x.Version.HasValue) // Omit results with invalid version strings
                .OrderByDescending(x => x.Version);
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

        /// <summary>
        ///     Extracts the version of the podcast as a decimal from the url property
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        private decimal? GetPodcastVersion(PodcastShow show) {
            var m = podcastUriMatcher.Match(show.Url);
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