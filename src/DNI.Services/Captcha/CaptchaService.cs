using System;
using System.Threading.Tasks;

using DNI.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RestSharp;

namespace DNI.Services.Captcha {
    /// <summary>
    ///     Provides a method for validating a CAPTCHA request with Google.
    /// </summary>
    public class CaptchaService : ICaptchaService {
        private readonly IRestClient _restClient;
        private readonly IOptions<CAPTCHAOptions> _captchaOptions;
        private readonly ILogger<CaptchaService> _logger;
        private readonly string _baseUrl = "https://www.google.com/";
        private readonly string _siteVerifyPath = "recaptcha/api/siteverify";

        public CaptchaService(IRestClient restClient, IOptions<CAPTCHAOptions> captchaOptions, ILogger<CaptchaService> logger) {
            _restClient = restClient;
            _captchaOptions = captchaOptions;
            _logger = logger;
        }

        public async Task<bool> VerifyAsync(string userResponse, string userIP) {
            if(userResponse == null) {
                throw new ArgumentNullException(nameof(userResponse), "userResponse cannot be null");
            }

            if(userIP == null) {
                throw new ArgumentNullException(nameof(userIP), "userIP cannot be null");
            }

            _restClient.BaseUrl = new Uri(_baseUrl);

            var request = new RestRequest(_siteVerifyPath) {
                Method = Method.POST
            };
            request.AddParameter("secret", _captchaOptions.Value.SecretKey, ParameterType.GetOrPost);
            request.AddParameter("response", userResponse, ParameterType.GetOrPost);
            request.AddParameter("remoteip", userIP, ParameterType.GetOrPost);

            _logger.LogDebug($"CAPTCHA: REQ URI: {request.Resource}");
            _logger.LogDebug($"CAPTCHA: REQ RESPONSE: {userResponse}");
            _logger.LogDebug($"CAPTCHA: REQ IP: {userIP}");
            var response = await _restClient.ExecuteAsync<SiteVerifyResponse>(request);

            if(response.ResponseStatus == ResponseStatus.Completed) {
                return response.Data.Success;
            }

            throw new Exception(string.Concat(response.ErrorMessage, "\r\nResponse content:\r\n", response.Content), response.ErrorException);
        }
    }
}