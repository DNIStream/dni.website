using System;
using System.Threading.Tasks;

using DNI.Options;

using Microsoft.Extensions.Options;

using RestSharp;

namespace DNI.Services.Vodcast {
    public class YouTubeVodcastService : IVodcastService {
        private readonly IRestClient _restClient;
        private readonly YouTubeOptions _youTubeOptions;
        private readonly GeneralOptions _options;

        public YouTubeVodcastService(IRestClient restClient, IOptions<GeneralOptions> options, IOptions<YouTubeOptions> youTubeOptions) {
            _restClient = restClient;
            _youTubeOptions = youTubeOptions.Value;
            _options = options.Value;
        }

        /// <summary>
        ///     Returns a <see cref="VodcastStream" /> object containing all published Vodcasts
        /// </summary>
        /// <returns></returns>
        public async Task<VodcastStream> GetAllAsync() {
            var url = $"playlistItems?part=snippet&maxResults=25&playlistId=PLlLn7y8D9PIg19rUENZ-WsOQCtClCwYob&key={_youTubeOptions.ApiKey}";
            _restClient.BaseUrl = new Uri(_options.VodcastServiceBaseUri);
            var request = new RestRequest {
                Method = Method.GET,
                Resource = url,
                RequestFormat = DataFormat.Json
            };

            // TODO: Parameterise
            request.AddHeader("Referrer", "http://localhost:4200");
            request.AddHeader("Origin", "http://localhost:4200");

            var response = await _restClient.ExecuteTaskAsync<VodcastStream>(request);
            return response.Data;
        }
    }
}