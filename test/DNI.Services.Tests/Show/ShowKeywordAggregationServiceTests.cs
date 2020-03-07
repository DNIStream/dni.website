using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Services.Podcast;
using DNI.Services.Show;
using DNI.Testing;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests.Show {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class ShowKeywordAggregationServiceTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        public ShowKeywordAggregationServiceTests(ITestOutputHelper output) {
            _output = output;
        }

        private IShowKeywordAggregationService GetService() {
            return new ShowKeywordAggregationService();
        }

        [Fact]
        public async Task GetKeywordDictionary_ReturnsAggregatedKeywords() {
            // Arrange
            var service = GetService();

            var show1 = _fixture.Build<PodcastShow>()
                .With(x => x.Keywords, () => new List<string> {
                    "tag1", "tag2", "tag3"
                })
                .Create();
            var show2 = _fixture.Build<PodcastShow>()
                .With(x => x.Keywords, () => new List<string> {
                    "tag2"
                })
                .Create();
            var show3 = _fixture.Build<PodcastShow>()
                .With(x => x.Keywords, () => new List<string> {
                    "tag3"
                })
                .Create();
            var show4 = _fixture.Build<PodcastShow>()
                .With(x => x.Keywords, () => new List<string> {
                    "tag3", "tag4"
                })
                .Create();
            var shows = new List<PodcastShow> {
                show1, show2, show3, show4
            };

            // Act
            var dictionary = await service.GetKeywordDictionaryAsync(shows);

            // Assert
            // tag1 = 1, tag2 = 2, tag3 = 3, tag4 = 1
            Assert.Equal(1, dictionary["tag1"]);
            Assert.Equal(2, dictionary["tag2"]);
            Assert.Equal(3, dictionary["tag3"]);
            Assert.Equal(1, dictionary["tag4"]);
        }

        [Fact]
        public async Task GetKeywordDictionary_ReturnsAggregatedKeywords_InDescendingCountOrder_ThenKey() {
            // Arrange
            var service = GetService();

            var show1 = _fixture.Build<PodcastShow>()
                .With(x => x.Keywords, () => new List<string> {
                    "tag1", "tag2", "tag3"
                })
                .Create();
            var show2 = _fixture.Build<PodcastShow>()
                .With(x => x.Keywords, () => new List<string> {
                    "tag2"
                })
                .Create();
            var show3 = _fixture.Build<PodcastShow>()
                .With(x => x.Keywords, () => new List<string> {
                    "tag3"
                })
                .Create();
            var show4 = _fixture.Build<PodcastShow>()
                .With(x => x.Keywords, () => new List<string> {
                    "tag3", "tag4"
                })
                .Create();
            var shows = new List<PodcastShow> {
                show1, show2, show3, show4
            };

            // Act
            var dictionary = await service.GetKeywordDictionaryAsync(shows);

            Assert.Equal(3, dictionary.ElementAt(0).Value);
            Assert.Equal("tag3", dictionary.ElementAt(0).Key);
            Assert.Equal(2, dictionary.ElementAt(1).Value);
            Assert.Equal("tag2", dictionary.ElementAt(1).Key);
            Assert.Equal(1, dictionary.ElementAt(2).Value);
            Assert.Equal("tag1", dictionary.ElementAt(2).Key);
            Assert.Equal(1, dictionary.ElementAt(3).Value);
            Assert.Equal("tag4", dictionary.ElementAt(3).Key);
        }
    }
}