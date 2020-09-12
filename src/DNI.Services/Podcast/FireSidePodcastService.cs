using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using DNI.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RestSharp;

namespace DNI.Services.Podcast {
    public class FiresidePodcastService : IPodcastService {
        private readonly IRestClient _restClient;
        private readonly ILogger<FiresidePodcastService> _logger;
        private readonly GeneralOptions _options;

        public FiresidePodcastService(IRestClient restClient, IOptions<GeneralOptions> options,
            ILogger<FiresidePodcastService> logger) {
            _restClient = restClient;
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        ///     Returns a <see cref="PodcastStream" /> object containing all published podcasts
        /// </summary>
        /// <returns></returns>
        public async Task<PodcastStream> GetAllAsync() {
            const string logPrefix = "FIRESIDE:";
            _logger.LogInformation($"{logPrefix} Start Communication");

            _restClient.BaseUrl = new Uri(_options.PodcastServiceBaseUri);
            _restClient.AddHandler("application/xml", () => new FiresideRssDeserializer());

            var request = new RestRequest {
                Method = Method.GET,
                Resource = _options.PodcastServiceResourceUri
            };

            _logger.LogInformation($"{logPrefix} Prepared {request.Method} request for Uri '{_restClient.BuildUri(request).AbsoluteUri}'");

            _logger.LogInformation($"{logPrefix} Sending request");

            var response = await _restClient.ExecuteGetAsync<PodcastStream>(request);

            _logger.LogInformation($"{logPrefix} Finished request. Response Uri is '{response.ResponseUri.AbsoluteUri}'");
            _logger.LogInformation($"{logPrefix} Response status: {response.StatusCode}");
            _logger.LogInformation($"{logPrefix} Response content length: {response.ContentLength}");

            // Only log the response data if its too small to be "complete" (10kb is sensible)
            // "-1" caters for a bug in RestSharp that seems to manifest itself when hosted in development mode
            if((response.ContentLength > -1 && response.ContentLength < 10240) || response.StatusCode != HttpStatusCode.OK) {
                _logger.LogInformation($"{logPrefix} Response content: {response.Content}");
                // Create an error entry
                var errorMessage = new StringBuilder($"{logPrefix} An error occurred when trying to retrieve data from the JSON endpoint:\n");
                errorMessage.AppendLine($"Status: {response.StatusCode}");
                errorMessage.AppendLine($"Content length: {response.ContentLength}");
                errorMessage.AppendLine($"Content: {response.Content}");
                _logger.LogError(errorMessage.ToString());
            }

            return response.Data;
        }
    }
}