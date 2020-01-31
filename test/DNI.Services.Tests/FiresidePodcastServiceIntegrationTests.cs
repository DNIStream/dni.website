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
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeIntegration)]
    public class FiresidePodcastServiceIntegrationTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
        private readonly IOptions<GeneralOptions> _generalOptions;
        private readonly Mock<ILogger<FiresidePodcastService>> _loggerMock;

        public FiresidePodcastServiceIntegrationTests(ITestOutputHelper output) {
            _output = output;

            _loggerMock = Mock.Get(_fixture.Create<ILogger<FiresidePodcastService>>());

            // Set up default / valid options
            var generalOptions = _fixture.Create<GeneralOptions>();
            _generalOptions = Microsoft.Extensions.Options.Options.Create(generalOptions);
            _generalOptions.Value.PodcastServiceResourceUri = "dnistream/rss";
            _generalOptions.Value.PodcastServiceBaseUri = "https://feeds.fireside.fm";
        }

        [Fact]
        public async Task GetAllAsync_ReturnsDataFromRemoteUri() {
            // Arrange
            var restClient = new RestClient();
            var service = new FiresidePodcastService(restClient, _generalOptions, _loggerMock.Object);

            // Act
            var r = await service.GetAllAsync();

            // Assert
            Assert.NotNull(r);
            Assert.NotNull(r.Shows);
            Assert.True(r.Shows.Count > 0);
        }
    }
}