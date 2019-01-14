using System.Linq;
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

        private VodcastStream vodcasts;
        private PodcastStream podcasts;

        public ShowListServiceTests(ITestOutputHelper output) {
            _output = output;

            vodcasts = _fixture.Create<VodcastStream>();
            podcasts = _fixture.Create<PodcastStream>();

            for(var i = 0; i < vodcasts.Shows.Count; i++) {
                vodcasts.Shows[i].Title = $"Documentation Not Included: Episode v{i+1}.0 - {vodcasts.Shows[i].Title}";
                podcasts.Shows[i].Url = $"https://podcast.dnistream.live/v{i+1}-0";
            }

            _vodcastClientMock = Mock.Get(_fixture.Create<IVodcastService>());
            _vodcastClientMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => vodcasts);

            _podcastClientMock = Mock.Get(_fixture.Create<IPodcastService>());
            _podcastClientMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => podcasts);
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
        public async Task GetShowsAsync_ReturnsMergedShows() {
            // Arrange
            var service = GetService();

            // Act
            var results = await service.GetShowsAsync();

            // Assert
            Assert.Equal(3, results.Count());
        }

        [Fact]
        public async Task GetShowsAsync_ReturnsAllPodcastResults_IfNoVodcastResults() {
            // Arrange
            _vodcastClientMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => null);
            var service = GetService();

            // Act
            var results = await service.GetShowsAsync();

            // Assert
            Assert.Equal(3, results.Count());
        }

        [Fact]
        public async Task GetShowsAsync_ReturnsAllVodcastResults_IfNoPodcastResults() {
            // Arrange
            _podcastClientMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => null);
            var service = GetService();

            // Act
            var results = await service.GetShowsAsync();

            // Assert
            Assert.Equal(3, results.Count());
        }

        [Fact]
        public async Task GetShowsAsync_OmitsVodcastsWithIncorrectTitle() {
            // Arrange
            var vodcastStream = _fixture.Create<VodcastStream>();
            _vodcastClientMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => vodcastStream);
            var service = GetService();

            // Act
            var results = await service.GetShowsAsync();

            // Assert
            Assert.Equal(3, results.Count());
        }

        [Fact]
        public async Task GetShowsAsync_OmitsPodcastsWithIncorrectUrl() {
            // Arrange
            var podcastStream = _fixture.Create<PodcastStream>();
            _podcastClientMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => podcastStream);
            var service = GetService();

            // Act
            var results = await service.GetShowsAsync();

            // Assert
            Assert.Equal(3, results.Count());
        }

        [Fact]
        public async Task GetShowsAsync_CreatesUniqueEntriesForValidButNonMatchingElements() {
            // Arrange

            // Just alter the first two entries sop they are valid, but don't match any other entries.
            // They should be added to the end of the results (2 matching results + 2 non matching results = 4 total records)
            vodcasts.Shows[0].Title = "Documentation Not Included: Episode v100.0 - Another Episode";
            podcasts.Shows[0].Url = "https://podcast.dnistream.live/v100-1";

            _vodcastClientMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => vodcasts);
            _podcastClientMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => podcasts);

            var service = GetService();

            // Act
            var results = await service.GetShowsAsync();

            // Assert
            Assert.Equal(4, results.Count());
        }
    }
}