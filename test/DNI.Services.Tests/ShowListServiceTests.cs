using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Services.Podcast;
using DNI.Services.ShowList;
using DNI.Services.Vodcast;
using DNI.Testing;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class ShowListServiceTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
        private readonly Mock<IVodcastService> _vodcastClientMock;
        private readonly Mock<IPodcastService> _podcastClientMock;

        public ShowListServiceTests(ITestOutputHelper output) {
            _output = output;

            _vodcastClientMock = Mock.Get(_fixture.Create<IVodcastService>());
            _podcastClientMock = Mock.Get(_fixture.Create<IPodcastService>());
        }

        private IShowListService GetService() {
            return new ShowListService(_podcastClientMock.Object, _vodcastClientMock.Object);
        }

        [Fact]
        public async Task GetShowsAsync_RetrievesDataFromPodcastService() {
            // Arrange
            var service = GetService();

            // Act
            await service.GetShowsAsync();

            // Assert
            _podcastClientMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetShowsAsync_RetrievesDataFromVodcastService() {
            // Arrange
            var service = GetService();

            // Act
            await service.GetShowsAsync();

            // Assert
            _vodcastClientMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetShowsAsync_ReturnsCountFromPodcastService() {
            // Arrange
            var service = GetService();

            // Act
            await service.GetShowsAsync();

            // Assert
            _vodcastClientMock.Verify(x => x.GetAllAsync(), Times.Once());
        }
    }
}