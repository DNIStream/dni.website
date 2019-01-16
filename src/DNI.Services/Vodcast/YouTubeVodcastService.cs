using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using DNI.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RestSharp;

namespace DNI.Services.Vodcast {
    public class YouTubeVodcastService : IVodcastService {
        private readonly IRestClient _restClient;
        private readonly ILogger<YouTubeVodcastService> _logger;
        private readonly YouTubeOptions _youTubeOptions;
        private readonly GeneralOptions _options;

        public YouTubeVodcastService(IRestClient restClient, IOptions<GeneralOptions> options,
            IOptions<YouTubeOptions> youTubeOptions, ILogger<YouTubeVodcastService> logger) {
            _restClient = restClient;
            _logger = logger;
            _youTubeOptions = youTubeOptions.Value;
            _options = options.Value;
        }

        /// <summary>
        ///     Returns a <see cref="VodcastStream" /> object containing all published Vodcasts
        /// </summary>
        /// <returns></returns>
        public async Task<VodcastStream> GetAllAsync() {
            var logPrefix = "YOUTUBE:";
            _logger.LogInformation($"{logPrefix} Start data retrieval");

            var url = $"playlistItems?part=snippet&maxResults=50&playlistId=PLlLn7y8D9PIg19rUENZ-WsOQCtClCwYob&key={_youTubeOptions.ApiKey}";
            _restClient.BaseUrl = new Uri(_options.VodcastServiceBaseUri);
            var request = new RestRequest {
                Method = Method.GET,
                Resource = url,
                RequestFormat = DataFormat.Json
            };

            request.AddHeader("Referrer", "http://localhost:4200");
            request.AddHeader("Origin", "http://localhost:4200");

            _logger.LogInformation($"{logPrefix} Prepared {request.Method.ToString()} request for Uri '{_restClient.BuildUri(request).AbsoluteUri}'");
            _logger.LogInformation($"{logPrefix} Sending API request");

            var response = await _restClient.ExecuteTaskAsync<VodcastStream>(request);

            _logger.LogInformation($"{logPrefix} Finished JSON request. Response Uri is '{response.ResponseUri.AbsoluteUri}'");
            _logger.LogInformation($"{logPrefix} Response status: {response.StatusCode.ToString()}");
            _logger.LogInformation($"{logPrefix} Response content length: {response.ContentLength}");

            // Only log the response data if its too small to be "complete" (10kb is sensible)
            // "-1" caters for a bug in RestSharp that seems to manifest itself when hosted in development mode
            if((response.ContentLength > -1 && response.ContentLength < 10240) || response.StatusCode != HttpStatusCode.OK) {
                _logger.LogInformation($"{logPrefix} Response content: {response.Content}");
                // Create an error entry
                var errorMessage = new StringBuilder($"{logPrefix} An error occurred when trying to retrieve data from the JSON endpoint:\n");
                errorMessage.AppendLine($"Status: {response.StatusCode.ToString()}");
                errorMessage.AppendLine($"Content length: {response.ContentLength}");
                errorMessage.AppendLine($"Content: {response.Content}");
                _logger.LogError(errorMessage.ToString());
            }

            return response.Data;
        }
    }
}