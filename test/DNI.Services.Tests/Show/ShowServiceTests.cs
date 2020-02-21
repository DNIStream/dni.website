using System;
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
using DNI.Services.Show;
using DNI.Testing;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests.Show {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class ShowServiceTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
        private readonly Mock<IPodcastService> _podcastServiceMock;
        private readonly Mock<IPagingCalculator<PodcastShow>> _pagingCalculatorMock;
        private readonly Mock<IShowKeywordAggregationService> _showKeywordAggregationServiceMock;
        private readonly Mock<IMapper<PodcastShow, Services.Show.Show>> _podcastShowMapperMock;
        private readonly Mock<ISorter<PodcastShow>> _sorterMock;
        private readonly Mock<ILogger<ShowService>> _loggerMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;

        public ShowServiceTests(ITestOutputHelper output) {
            _output = output;

            _loggerMock = Mock.Get(_fixture.Create<ILogger<ShowService>>());

            _showKeywordAggregationServiceMock = Mock.Get(_fixture.Create<IShowKeywordAggregationService>());
            _pagingCalculatorMock = Mock.Get(_fixture.Create<IPagingCalculator<PodcastShow>>());
            _podcastShowMapperMock = Mock.Get(_fixture.Create<IMapper<PodcastShow, Services.Show.Show>>());
            _sorterMock = Mock.Get(_fixture.Create<ISorter<PodcastShow>>());
            _memoryCacheMock = Mock.Get(_fixture.Create<IMemoryCache>());
            _podcastServiceMock = Mock.Get(_fixture.Create<IPodcastService>());

            _pagingCalculatorMock
                .Setup(x => x.PageItemsAsync<PodcastShowPagedResponse>(It.IsAny<PodcastShow[]>(), It.IsAny<IPagingRequest>()))
                .ReturnsAsync(() => _fixture.Create<PodcastShowPagedResponse>());
        }

        private IShowService GetService() {
            return new ShowService(_podcastServiceMock.Object, _showKeywordAggregationServiceMock.Object,
                _pagingCalculatorMock.Object, _podcastShowMapperMock.Object, _sorterMock.Object, _loggerMock.Object,
                _memoryCacheMock.Object);
        }

        #region GetShowsAsync

        [Fact]
        public async Task GetShowListAsync_RetrievesDataFromPodcastService() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();

            // Act
            await service.GetShowsAsync(request, request);

            // Assert
            _podcastServiceMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetShowListAsync_ReturnsNullWhenPodcastServiceReturnsNull() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            _podcastServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => null);

            // Act
            var response = await service.GetShowsAsync(request, request);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetShowListAsync_ReturnsNullWhenPodcastsAreNull() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            _podcastServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => new PodcastStream {
                    Shows = null
                });

            // Act
            var response = await service.GetShowsAsync(request, request);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetShowListAsync_ReturnsNullWhenNoPodcastsArePresent() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            _podcastServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => new PodcastStream {
                    Shows = new List<PodcastShow>()
                });

            // Act
            var response = await service.GetShowsAsync(request, request);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetShowListAsync_CallsSorter_WithEnumeratedPodcastShows_AndSortingRequest() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            var show1 = _fixture.Create<PodcastShow>();
            var show2 = _fixture.Create<PodcastShow>();
            var show3 = _fixture.Create<PodcastShow>();

            _podcastServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => new PodcastStream {
                    Shows = {
                        show1, show2, show3
                    }
                });

            // Act
            await service.GetShowsAsync(request, request);

            // Assert
            _sorterMock
                .Verify(x => x.SortAsync(It.Is<IEnumerable<PodcastShow>>(k => k.Contains(show1) && k.Contains(show2) && k.Contains(show3)),
                    It.Is<ISortingRequest>(r => r == request)), Times.Once());
        }

        [Fact]
        public async Task GetShowListAsync_MapsOnlyPagedPodcastShowsToAShow() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            var podcastShows = _fixture.CreateMany<PodcastShow>(6).ToArray();
            var podcastShowPagedResponse = _fixture
                .Build<PodcastShowPagedResponse>()
                .With(x => x.Items, podcastShows)
                .Create();
            _pagingCalculatorMock
                .Setup(x => x.PageItemsAsync<PodcastShowPagedResponse>(It.IsAny<PodcastShow[]>(), It.IsAny<IPagingRequest>()))
                .ReturnsAsync(() => podcastShowPagedResponse);

            // Act
            (await service.GetShowsAsync(request, request)).Items.ToArray();

            // Assert
            _podcastShowMapperMock
                .Verify(x => x.Map(It.IsIn(podcastShows)), Times.Exactly(6));
        }

        [Fact]
        public async Task GetShowListAsync_ReturnsExpectedMappedShows() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            var mappedShows = _fixture.CreateMany<Services.Show.Show>(3).ToArray();

            _podcastShowMapperMock
                .SetupSequence(x => x.Map(It.IsAny<PodcastShow>()))
                .Returns(() => mappedShows[0])
                .Returns(() => mappedShows[1])
                .Returns(() => mappedShows[2]);

            // Act
            var items = (await service.GetShowsAsync(request, request)).Items.ToArray();

            // Assert
            Assert.Equal(mappedShows.Length, items.Length);
            Assert.Equal(mappedShows[0], items.ElementAt(0));
            Assert.Equal(mappedShows[1], items.ElementAt(1));
            Assert.Equal(mappedShows[2], items.ElementAt(2));
        }

        [Fact]
        public async Task GetShowListAsync_ReturnsExpectedPagingInfo() {
            // Arrange
            var service = GetService();
            var request = new GetShowsRequest();
            var expectedPagedItemsReponse = _fixture.Create<PodcastShowPagedResponse>();

            _pagingCalculatorMock
                .Setup(x => x.PageItemsAsync<PodcastShowPagedResponse>(It.IsAny<PodcastShow[]>(), It.IsAny<IPagingRequest>()))
                .ReturnsAsync(() => expectedPagedItemsReponse);

            // Act
            var actualResponse = await service.GetShowsAsync(request, request);

            // Assert
            Assert.Equal(expectedPagedItemsReponse.CurrentPage, actualResponse.CurrentPage);
            Assert.Equal(expectedPagedItemsReponse.EndIndex, actualResponse.EndIndex);
            Assert.Equal(expectedPagedItemsReponse.StartIndex, actualResponse.StartIndex);
            Assert.Equal(expectedPagedItemsReponse.TotalPages, actualResponse.TotalPages);
            Assert.Equal(expectedPagedItemsReponse.TotalRecords, actualResponse.TotalRecords);
            Assert.Equal(expectedPagedItemsReponse.ItemsPerPage, actualResponse.ItemsPerPage);
        }

        #endregion

        #region GetLatestShowAsync

        [Fact]
        public async Task GetLatestShowAsync_GetsAllShowsFromPodcastService() {
            // Arrange
            var service = GetService();

            // Act
            await service.GetLatestShowAsync();

            // Assert
            _podcastServiceMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetLatestShowAsync_MapsOnlyLatestShowReturnedFromPodcastService() {
            // Arrange
            var service = GetService();
            var expectedShows = _fixture.CreateMany<PodcastShow>(3).ToList();
            expectedShows[0].PublishedTime = new DateTime(2020, 1, 1, 3, 45, 23); // Second
            expectedShows[1].PublishedTime = new DateTime(2020, 5, 6, 18, 29, 1); // First
            expectedShows[2].PublishedTime = new DateTime(2019, 12, 5); // Third

            var expectedPodcastStream = _fixture
                .Build<PodcastStream>()
                .With(x => x.Shows, expectedShows)
                .Create();
            _podcastServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => expectedPodcastStream);

            // Act
            await service.GetLatestShowAsync();

            // Assert
            _podcastShowMapperMock
                .Verify(x => x.Map(It.Is<PodcastShow>(s => s == expectedShows[1])), Times.Once());
        }

        #endregion

        #region GetShowBySlugAsync

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\t")]
        public async Task GetShowBySlug_ThrowsArgumentNullException_IfSlugIsNotPassed(string slug) {
            // Arrange
            var service = GetService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetShowBySlugAsync(slug));
            Assert.Equal("slug", ex.ParamName);
        }

        [Fact]
        public async Task GetShowBySlug_CallsGetAllAsyncOnPodcastService_IfSlugIsValidString() {
            // Arrange
            var service = GetService();
            var slug = (GetShowBySlug_SetupValidPodcastShowList()).Slug;

            // Act
            await service.GetShowBySlugAsync(slug);

            // Assert
            _podcastServiceMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetShowBySlug_MapsShowWithRequestedSlug() {
            // Arrange
            var service = GetService();
            var expectedShow = GetShowBySlug_SetupValidPodcastShowList();

            // Act
            await service.GetShowBySlugAsync(expectedShow.Slug);

            // Assert
            _podcastShowMapperMock
                .Verify(x => x.Map(It.Is<PodcastShow>(s => s == expectedShow)),
                    Times.Once());
        }

        [Fact]
        public async Task GetShowBySlug_ReturnsMappedShow() {
            // Arrange
            var service = GetService();
            var show = GetShowBySlug_SetupValidPodcastShowList();
            var expectedShow = _fixture.Create<Services.Show.Show>();

            _podcastShowMapperMock
                .Setup(x => x.Map(It.IsAny<PodcastShow>()))
                .Returns(() => expectedShow);

            // Act
            var actualShow = await service.GetShowBySlugAsync(show.Slug);

            // Assert
            Assert.Equal(expectedShow, actualShow);
        }

        [Fact]
        public async Task GetShowBySlug_ThrowsInvalidOperationException_WhenSlugDoesNotExist() {
            // Arrange
            var service = GetService();
            var expectedSlug = _fixture.Create<string>();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetShowBySlugAsync(expectedSlug));
            Assert.Contains($"Slug '{expectedSlug}' not found", ex.Message);
        }

        #endregion

        #region GetAggregatedKeywords

        [Fact]
        public async Task GetAggregatedKeywords_CallsPodcastService() {
            // Arrange
            var service = GetService();

            // Act
            await service.GetAggregatedKeywords();

            // Assert
            _podcastServiceMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetAggregatedKeywords_PassesAllPodcastResults_ToKeywordAggregationService() {
            // Arrange
            var service = GetService();
            var podcastShows = _fixture.CreateMany<PodcastShow>(3);
            _podcastServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => new PodcastStream {
                    Shows = podcastShows.ToList()
                });

            // Act
            await service.GetAggregatedKeywords();

            // Assert
            _showKeywordAggregationServiceMock
                .Verify(x => x.GetKeywordDictionaryAsync(It.Is<IEnumerable<PodcastShow>>(s => s.Contains(podcastShows.ElementAt(0)))), Times.Once());
            _showKeywordAggregationServiceMock
                .Verify(x => x.GetKeywordDictionaryAsync(It.Is<IEnumerable<PodcastShow>>(s => s.Contains(podcastShows.ElementAt(1)))), Times.Once());
            _showKeywordAggregationServiceMock
                .Verify(x => x.GetKeywordDictionaryAsync(It.Is<IEnumerable<PodcastShow>>(s => s.Contains(podcastShows.ElementAt(2)))), Times.Once());
        }

        [Fact]
        public async Task GetAggregatedKeywords_ReturnsKeywordsFromKeywordAggregationService() {
            // Arrange
            var service = GetService();
            var keywords = _fixture.Create<IDictionary<string, int>>();

            _showKeywordAggregationServiceMock
                .Setup(x => x.GetKeywordDictionaryAsync(It.IsAny<IEnumerable<PodcastShow>>()))
                .ReturnsAsync(() => keywords);

            // Act
            var result = await service.GetAggregatedKeywords();

            // Assert
            Assert.Equal(keywords, result);
        }

        #endregion

        /// <summary>
        ///     Helper method. Sets up GetAllAsync() on the <see cref="_podcastServiceMock" /> with the specified number of
        ///     <see cref="PodcastShow" />s, then returns the <see cref="PodcastShow" /> at the specified
        ///     <paramref name="returnShowIndex" />
        /// </summary>
        /// <param name="countOfShows"></param>
        /// <param name="returnShowIndex"></param>
        /// <returns></returns>
        private PodcastShow GetShowBySlug_SetupValidPodcastShowList(int countOfShows = 3, int returnShowIndex = 0) {
            var allShows = _fixture
                .Build<PodcastShow>()
                .With(x => x.PageUrl, $"https://test.com/{_fixture.Create<string>()}")
                .CreateMany(countOfShows)
                .ToList();

            _podcastServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => new PodcastStream {
                    Shows = allShows
                });
            return allShows[returnShowIndex];
        }
    }
}