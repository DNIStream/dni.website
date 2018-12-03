using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DNI.Services.Podcast;
using DNI.Services.Vodcast;

namespace DNI.Services.ShowList {
    public class ShowListService : IShowListService {
        private readonly IPodcastService _podcastService;
        private readonly IVodcastService _vodcastService;

        public ShowListService(IPodcastService podcastService, IVodcastService vodcastService) {
            _podcastService = podcastService;
            _vodcastService = vodcastService;
        }

        /// <summary>
        ///     Retrieves aggregated vodcast and podcast shows, ordered in descending published date order.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Show>> GetShowsAsync() {
            throw new NotImplementedException();
        }
    }
}