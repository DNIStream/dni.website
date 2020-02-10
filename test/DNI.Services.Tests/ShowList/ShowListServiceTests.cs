using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.API.Requests;
using DNI.Services.Podcast;
using DNI.Services.Shared.Mapping;
using DNI.Services.Shared.Paging;
using DNI.Services.Shared.Sorting;
using DNI.Services.ShowList;
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
        private readonly Mock<IPodcastService> _podcastServiceMock;
        private readonly Mock<IPagingCalculator<Show>> _pagingCalculatorMock;
        private readonly Mock<IShowKeywordAggregationService> _showKeywordAggregationServiceMock;
        private readonly Mock<IMapper<PodcastShow, Show>> _podcastShowMapperMock;
        private readonly Mock<ISorter> _sorterMock;
        private readonly Mock<ILogger<ShowListService>> _loggerMock;

        private readonly PodcastStream podcasts;

        public ShowListServiceTests(ITestOutputHelper output) {
            _output = output;

            _loggerMock = Mock.Get(_fixture.Create<ILogger<ShowListService>>());

            podcasts = _fixture.Create<PodcastStream>();

            _showKeywordAggregationServiceMock = Mock.Get(_fixture.Create<IShowKeywordAggregationService>());
            _pagingCalculatorMock = Mock.Get(_fixture.Create<IPagingCalculator<Show>>());
            _podcastShowMapperMock = Mock.Get(_fixture.Create<IMapper<PodcastShow, Show>>());
            _sorterMock = Mock.Get(_fixture.Create<ISorter>());

            _podcastServiceMock = Mock.Get(_fixture.Create<IPodcastService>());
            _podcastServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => podcasts);
        }

        private IShowListService GetService() {
            return new ShowListService(_podcastServiceMock.Object, _showKeywordAggregationServiceMock.Object,
                _pagingCalculatorMock.Object, _podcastShowMapperMock.Object, _sorterMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetShowListAsync_RetrievesDataFromPodcastService() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();

            // Act
            await service.GetShowListAsync(request, request);

            // Assert
            _podcastServiceMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetShowListAsync_MapsEachPodcastShowToAShow() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            var podcastShows = _fixture.CreateMany<PodcastShow>(6).ToList();
            _podcastServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => new PodcastStream {
                    Shows = podcastShows
                });

            // Act
            await service.GetShowListAsync(request, request);

            // Assert
            _podcastShowMapperMock
                .Verify(x => x.Map(It.IsIn<PodcastShow>(podcastShows)), Times.Exactly(6));
        }

        [Fact]
        public async Task GetShowListAsync_ReturnsExpectedPagedResponse_AndExpectedNumberOfShows() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            var shows = _fixture.CreateMany<Show>(34);
            var sortingResponse = _fixture.CreateMany<Show>().ToArray();
            var pagedResponse = _fixture
                .Build<Services.ShowList.ShowList>()
                .With(x => x.Items, shows)
                .Create();
            _sorterMock
                .Setup(x => x.SortAsync(It.IsAny<IEnumerable<Show>>(), It.IsAny<ISortingInfo>()))
                .ReturnsAsync(sortingResponse);

            _pagingCalculatorMock
                .Setup(x => x.PageItemsAsync<Services.ShowList.ShowList>(It.Is<IPagingInfo>(r => r == request),
                    It.Is<Show[]>(s => Equals(s, sortingResponse))))
                .ReturnsAsync(() => pagedResponse)
                .Verifiable();

            // Act
            var actualResponse = await service.GetShowListAsync(request, request);

            // Assert
            _pagingCalculatorMock.Verify();
            Assert.Equal(pagedResponse, actualResponse);
            Assert.Equal(shows.Count(), actualResponse.Items.Count());
        }

        [Fact]
        public async Task GetShowListAsync_CallsGetKeywordDictionaryAsync_WithEnumeratedShows() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            var show1 = _fixture.Create<Show>();
            var show2 = _fixture.Create<Show>();
            var show3 = _fixture.Create<Show>();

            _podcastShowMapperMock
                .SetupSequence(x => x.Map(It.IsAny<PodcastShow>()))
                .Returns(show1)
                .Returns(show2)
                .Returns(show3);

            // Act
            await service.GetShowListAsync(request, request);

            // Assert
            _showKeywordAggregationServiceMock
                .Verify(x => x.GetKeywordDictionaryAsync(It.Is<IEnumerable<Show>>(k => k.Contains(show1) && k.Contains(show2) && k.Contains(show3))),
                    Times.Once());
        }

        [Fact]
        public async Task GetShowListAsync_ReturnsKeywordsFromKeywordAggregationService() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            var keywords = _fixture.Create<IDictionary<string, int>>();

            _showKeywordAggregationServiceMock
                .Setup(x => x.GetKeywordDictionaryAsync(It.IsAny<IEnumerable<Show>>()))
                .ReturnsAsync(() => keywords);

            // Act
            var result = await service.GetShowListAsync(request, request);

            // Assert
            Assert.Equal(keywords, result.TotalKeywordCounts);
        }

        [Fact]
        public async Task GetShowListAsync_CallsSorter_WithEnumeratedShows_AndSortingRequest() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            var show1 = _fixture.Create<Show>();
            var show2 = _fixture.Create<Show>();
            var show3 = _fixture.Create<Show>();

            _podcastShowMapperMock
                .SetupSequence(x => x.Map(It.IsAny<PodcastShow>()))
                .Returns(show1)
                .Returns(show2)
                .Returns(show3);

            // Act
            await service.GetShowListAsync(request, request);

            // Assert
            _sorterMock
                .Verify(x => x.SortAsync(It.Is<IEnumerable<Show>>(k => k.Contains(show1) && k.Contains(show2) && k.Contains(show3)),
                    It.Is<ISortingInfo>(r => r == request)), Times.Once());
        }
    }
}