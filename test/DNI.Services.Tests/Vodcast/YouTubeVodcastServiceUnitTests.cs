using System;
using System.Net;
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

namespace DNI.Services.Tests.Vodcast {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class YouTubeVodcastServiceUnitTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
        private readonly Mock<IRestClient> _restClientMock;
        private readonly Mock<IOptions<GeneralOptions>> _generalOptions;
        private readonly Mock<IOptions<YouTubeOptions>> _youTubeOptions;
        private readonly Mock<ILogger<YouTubeVodcastService>> _loggerMock;

        public YouTubeVodcastServiceUnitTests(ITestOutputHelper output) {
            _output = output;

            _fixture.Register(() => new CookieContainer(1));
            _fixture.Register(() => new Uri("http://www.test.com"));
            _restClientMock = Mock.Get(_fixture.Create<IRestClient>());
            _restClientMock
                .Setup(x => x.BuildUri(It.IsAny<IRestRequest>()))
                .Returns(() => new Uri("http://www.test.com"));

            _loggerMock = Mock.Get(_fixture.Create<ILogger<YouTubeVodcastService>>());

            _generalOptions = Mock.Get(_fixture.Create<IOptions<GeneralOptions>>());
            _youTubeOptions = Mock.Get(_fixture.Create<IOptions<YouTubeOptions>>());
        }

        private IVodcastService GetService() {
            _generalOptions.Object.Value.VodcastServiceBaseUri = "https://awebsite";
            return new YouTubeVodcastService(_restClientMock.Object, _generalOptions.Object, _youTubeOptions.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_CallsRESTClientWithInjectedDataUrl() {
            // Arrange
            _restClientMock
                .Setup(x => x.ExecuteTaskAsync<VodcastStream>(It.IsAny<RestRequest>()))
                .ReturnsAsync(() => _fixture.Create<IRestResponse<VodcastStream>>());
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