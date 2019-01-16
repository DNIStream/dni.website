using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Options;
using DNI.Services.Podcast;
using DNI.Testing;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using RestSharp;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class FiresidePodcastServiceUnitTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<IRestClient> _restClientMock;
        private readonly Mock<IOptions<GeneralOptions>> _generalOptions;
        private readonly Mock<ILogger<FiresidePodcastService>> _loggerMock;

        public FiresidePodcastServiceUnitTests(ITestOutputHelper output) {
            _output = output;

            _restClientMock = Mock.Get(_fixture.Create<IRestClient>());
            _generalOptions = Mock.Get(_fixture.Create<IOptions<GeneralOptions>>());
            _loggerMock = Mock.Get(_fixture.Create<ILogger<FiresidePodcastService>>());
        }

        private IPodcastService GetService() {
            _generalOptions.Object.Value.PodcastServiceBaseUri = "https://awebsite.com";
            return new FiresidePodcastService(_restClientMock.Object, _generalOptions.Object, _loggerMock.Object);
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
                    r.Resource == _generalOptions.Object.Value.PodcastServiceResourceUri
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