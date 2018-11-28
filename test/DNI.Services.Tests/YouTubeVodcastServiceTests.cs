using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Options;
using DNI.Services.Vodcast;

using Microsoft.Extensions.Options;

using Moq;

using RestSharp;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests {
    [Trait("TestType", "Unit")]
    public class YouTubeVodcastServiceTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<IRestClient> _restClientMock;
        private readonly IOptions<GeneralOptions> _generalOptions;
        private readonly IOptions<YouTubeOptions> _youTubeOptions;

        public YouTubeVodcastServiceTests(ITestOutputHelper output) {
            _output = output;

            _restClientMock = Mock.Get(_fixture.Create<IRestClient>());

            // Set up default / valid options
            var generalOptions = _fixture.Create<GeneralOptions>();
            _generalOptions = Microsoft.Extensions.Options.Options.Create(generalOptions);
            _generalOptions.Value.VodcastServiceBaseUri = "https://www.googleapis.com/youtube/v3";

            var youTubeOptions = _fixture.Create<YouTubeOptions>();
            _youTubeOptions = Microsoft.Extensions.Options.Options.Create(youTubeOptions);
            _youTubeOptions.Value.ApiKey = ""; // Add API key to run integration tests (but don't check in!)
        }

        private IVodcastService GetService() {
            return new YouTubeVodcastService(_restClientMock.Object, _generalOptions, _youTubeOptions);
        }

        /// <summary>
        ///     The below test will only succeed if an APIKey is provided
        /// </summary>
        /// <returns></returns>
        [Trait("TestType", "Integration")]
        [Fact]
        public async Task GetAllAsync_ReturnsDataFromRemoteUri() {
            // Arrange
            var restClient = new RestClient();
            var service = new YouTubeVodcastService(restClient, _generalOptions, _youTubeOptions);

            // Act
            var r = await service.GetAllAsync();

            // Assert
            Assert.NotNull(r.Shows);
            Assert.True(r.Shows.Count > 0);
        }

        [Fact]
        public async Task GetAllAsync_CallsRESTClientWithInjectedDataUrl() {
            // Arrange
            var service = GetService();

            // Act
            await service.GetAllAsync();

            // Assert
            _restClientMock
                .Verify(x => x.ExecuteTaskAsync<VodcastStream>(It.Is<RestRequest>(r =>
                    r.Resource.StartsWith("playlistItems")
                )), Times.Once(), "Vodcast Service Resource Uri expected");
            _restClientMock
                .Verify(x => x.ExecuteTaskAsync<VodcastStream>(It.Is<RestRequest>(r =>
                    r.RequestFormat == DataFormat.Json
                )), Times.Once(), "Vodcast Service Data format should be JSON");
            _restClientMock
                .Verify(x => x.ExecuteTaskAsync<VodcastStream>(It.Is<RestRequest>(r =>
                    r.Method == Method.GET
                )), Times.Once(), "GET Expected");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSerializedData_FromExecuteTaskAsync() {
            // Arrange
            var expectedResult = _fixture.Create<IRestResponse<VodcastStream>>();
            _restClientMock
                .Setup(x => x.ExecuteTaskAsync<VodcastStream>(It.IsAny<RestRequest>()))
                .ReturnsAsync(() => expectedResult);

            var service = GetService();

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(expectedResult.Data, result);
        }
    }
}