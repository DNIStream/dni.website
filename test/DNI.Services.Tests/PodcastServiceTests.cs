using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Options;
using DNI.Services.Podcast;

using Microsoft.Extensions.Options;

using Moq;

using RestSharp;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests {
    [Trait("TestType", "Unit")]
    public class PodcastServiceTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<IRestClient> _restClientMock;
        private readonly IOptions<GeneralOptions> _generalOptions;

        public PodcastServiceTests(ITestOutputHelper output) {
            _output = output;

            _restClientMock = Mock.Get(_fixture.Create<IRestClient>());

            // Set up default / valid options
            var generalOptions = _fixture.Create<GeneralOptions>();
            _generalOptions = Microsoft.Extensions.Options.Options.Create(generalOptions);
            _generalOptions.Value.PodcastServiceResourceUri = "json";
            _generalOptions.Value.PodcastServiceBaseUri = "https://podcast.dnistream.live";
        }

        private IPodcastService GetService() {
            return new PodcastService(_restClientMock.Object, _generalOptions);
        }

        [Trait("TestType", "Integration")]
        [Fact]
        public async Task GetAllAsync_ReturnsDataFromRemoteUri() {
            // Arrange
            var restClient = new RestClient();
            var service = new PodcastService(restClient, _generalOptions);

            // Act
            var r = await service.GetAllAsync();

            // Assert
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
                .Verify(x => x.ExecuteTaskAsync<PodcastStream>(It.Is<RestRequest>(r =>
                    r.Resource == _generalOptions.Value.PodcastServiceResourceUri
                )), Times.Once(), "Podcast Service Resource Uri expected");
            _restClientMock
                .Verify(x => x.ExecuteTaskAsync<PodcastStream>(It.Is<RestRequest>(r =>
                    r.RequestFormat == DataFormat.Json
                )), Times.Once(), "Podcast Service Data format should be JSON");
            _restClientMock
                .Verify(x => x.ExecuteTaskAsync<PodcastStream>(It.Is<RestRequest>(r =>
                    r.Method == Method.GET
                )), Times.Once(), "GET Expected");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSerializedData_FromExecuteTaskAsync() {
            // Arrange
            var expectedResult = _fixture.Create<IRestResponse<PodcastStream>>();
            _restClientMock
                .Setup(x => x.ExecuteTaskAsync<PodcastStream>(It.IsAny<RestRequest>()))
                .ReturnsAsync(() => expectedResult);

            var service = GetService();

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(expectedResult.Data, result);
        }
    }
}