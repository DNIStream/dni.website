using System;
using System.Threading.Tasks;

using DNI.Options;

using Microsoft.Extensions.Options;

using RestSharp;

namespace DNI.Services.Podcast {
    public class FiresidePodcastService : IPodcastService {
        private readonly IRestClient _restClient;
        private readonly GeneralOptions _options;

        public FiresidePodcastService(IRestClient restClient, IOptions<GeneralOptions> options) {
            _restClient = restClient;
            _options = options.Value;
        }

        /// <summary>
        ///     Returns a <see cref="PodcastStream" /> object containing all published podcasts
        /// </summary>
        /// <returns></returns>
        public async Task<PodcastStream> GetAllAsync() {
            _restClient.BaseUrl = new Uri(_options.PodcastServiceBaseUri);
            var request = new RestRequest {
                Method = Method.GET,
                Resource = _options.PodcastServiceResourceUri,
                RequestFormat = DataFormat.Json
            };

            var response = await _restClient.ExecuteTaskAsync<PodcastStream>(request);
            return response.Data;
        }
    }
}