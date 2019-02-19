using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Options;
using DNI.Services.Vodcast;
using DNI.Testing;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using RestSharp;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeIntegration)]
    public class YouTubeVodcastServiceIntegrationTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
        private readonly IOptions<GeneralOptions> _generalOptions;
        private readonly IOptions<YouTubeOptions> _youTubeOptions;
        private readonly Mock<ILogger<YouTubeVodcastService>> _loggerMock;

        public YouTubeVodcastServiceIntegrationTests(ITestOutputHelper output) {
            _output = output;

            _loggerMock = Mock.Get(_fixture.Create<ILogger<YouTubeVodcastService>>());

            // Set up default / valid options
            var generalOptions = _fixture.Create<GeneralOptions>();
            _generalOptions = Microsoft.Extensions.Options.Options.Create(generalOptions);
            _generalOptions.Value.VodcastServiceBaseUri = "https://www.googleapis.com/youtube/v3";

            var youTubeOptions = _fixture.Create<YouTubeOptions>();
            _youTubeOptions = Microsoft.Extensions.Options.Options.Create(youTubeOptions);
            _youTubeOptions.Value.ApiKey = TestHelpers.GetKeyValue("YOUTUBE");
        }

        /// <summary>
        ///     The below test will only succeed if an APIKey is provided
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllAsync_ReturnsDataFromRemoteUri() {
            // Arrange
            var restClient = new RestClient();
            var service = new YouTubeVodcastService(restClient, _generalOptions, _youTubeOptions, _loggerMock.Object);

            // Act
            var r = await service.GetAllAsync();

            // Assert
            Assert.NotNull(r.Shows);
            Assert.True(r.Shows.Count > 0);
        }
    }
}