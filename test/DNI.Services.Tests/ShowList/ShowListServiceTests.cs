using System.Linq;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Services.Podcast;
using DNI.Services.ShowList;
using DNI.Services.Vodcast;
using DNI.Testing;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests.ShowList {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class ShowListServiceTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
        private readonly Mock<IPodcastService> _podcastClientMock;
        private readonly Mock<ILogger<ShowListService>> _loggerMock;

        private readonly PodcastStream podcasts;

        public ShowListServiceTests(ITestOutputHelper output) {
            _output = output;

            _loggerMock = Mock.Get(_fixture.Create<ILogger<ShowListService>>());

            podcasts = _fixture.Create<PodcastStream>();

            _podcastClientMock = Mock.Get(_fixture.Create<IPodcastService>());
            _podcastClientMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => podcasts);
        }

        private IShowListService GetService() {
            return new ShowListService(_podcastClientMock.Object, _loggerMock.Object);
        }

        #region GetShowListAsync

        [Fact]
        public async Task GetShowsAsync_RetrievesDataFromPodcastService() {
            // Arrange
            var service = GetService();

            // Act
            await service.GetShowListAsync();

            // Assert
            _podcastClientMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetShowsAsync_ReturnsExpectedPodcastShows() {
            // Arrange
            var service = GetService();

            // Act
            var results = await service.GetShowListAsync();

            // Assert
            Assert.Equal(3, results.Shows.Count());
        }

        #endregion

        #region GetShowListAsync(field, order)

        [Fact]
        public async Task GetShowsAsyncOrdered_ReturnsExpectedCountOfShows() {
            // Arrange
            var service = GetService();

            // Act
            var results = await service.GetShowListAsync(ShowOrderField.PublishedTime, ShowOrderFieldOrder.Descending);

            // Assert
            Assert.Equal(3, results.Shows.Count());
        }

        [Fact]
        public async Task GetShowsAsyncOrdered_ReturnsShowsInShowDateDescendingOrder() {
            // Arrange
            var service = GetService();

            // Act
            var results = (await service.GetShowListAsync(ShowOrderField.PublishedTime, ShowOrderFieldOrder.Descending)).Shows.ToArray();

            // Assert
            Assert.True(results.SequenceEqual(results.OrderByDescending(s => s.PublishedTime)));
        }

        [Fact]
        public async Task GetShowsAsyncOrdered_ReturnsShowsInShowDateAscendingOrder() {
            // Arrange
            var service = GetService();

            // Act
            var results = (await service.GetShowListAsync(ShowOrderField.PublishedTime, ShowOrderFieldOrder.Ascending)).Shows.ToArray();

            // Assert
            Assert.True(results.SequenceEqual(results.OrderBy(s => s.PublishedTime)));
        }

        [Fact]
        public async Task GetShowsAsyncOrdered_ReturnsShowsInVersionDescendingOrder() {
            // Arrange
            var service = GetService();

            // Act
            var results = (await service.GetShowListAsync(ShowOrderField.Version, ShowOrderFieldOrder.Descending)).Shows.ToArray();

            // Assert
            Assert.True(results.SequenceEqual(results.OrderByDescending(s => s.Version)));
        }

        [Fact]
        public async Task GetShowsAsyncOrdered_ReturnsShowsInVersionAscendingOrder() {
            // Arrange
            var service = GetService();

            // Act
            var results = (await service.GetShowListAsync(ShowOrderField.Version, ShowOrderFieldOrder.Ascending)).Shows.ToArray();

            // Assert
            Assert.True(results.SequenceEqual(results.OrderBy(s => s.Version)));
        }

        #endregion
    }
}