using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Services.Podcast;
using DNI.Testing;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests.Podcast {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class PodcastShowUnitTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        public PodcastShowUnitTests(ITestOutputHelper output) {
            _output = output;
        }

        [Theory]
        [InlineData("https://test.com/v0-1", "0.1")]
        [InlineData("https://test.com/v13-2", "13.2")]
        [InlineData("http://test.co.uk/shows/v3-56", "3.56")]
        [InlineData("http://test.co.uk/shows/dni/v456-2223", "456.2223")]
        [InlineData("http://test.co.uk/shows/dni/V2-4", "2.4")]
        public void Version_ReturnsVersionFromVersionFormattedPageUrl(string inputUrl, string expectedVersion) {
            // Arrange
            var show = new PodcastShow {
                PageUrl = inputUrl
            };

            // Act
            var actualVersion = show.Version;

            // Assert
            Assert.NotNull(actualVersion);
            Assert.Equal(expectedVersion, actualVersion);
        }

        [Theory]
        [InlineData("https://test.com/0-1")]
        [InlineData("https://test.com/test")]
        [InlineData("http://test.co.uk/shows/356")]
        [InlineData("http://test.co.uk/shows/dni/456-2223")]
        [InlineData("http://test.co.uk/shows/dni/v2.4")]
        [InlineData("http://test.co.uk/shows/dni/v2-4/")]
        public void Version_ReturnsNullFromIncorrectlyFormattedPageUrl(string inputUrl) {
            // Arrange
            var show = new PodcastShow {
                PageUrl = inputUrl
            };

            // Act
            var actualVersion = show.Version;

            // Assert
            Assert.Null(actualVersion);
        }

        [Theory]
        [InlineData("https://test.com/v0-1", "v0-1")]
        [InlineData("https://test.com/v13-2", "v13-2")]
        [InlineData("http://test.co.uk/shows/v3-56", "v3-56")]
        [InlineData("http://test.co.uk/shows/dni/v456-2223", "v456-2223")]
        [InlineData("http://test.co.uk/shows/dni/V2-4", "v2-4")]
        public void Slug_ReturnsSlugFromFormattedPageUrl(string inputUrl, string expectedSlug) {
            // Arrange
            var show = new PodcastShow {
                PageUrl = inputUrl
            };

            // Act
            var actualSlug = show.Slug;

            // Assert
            Assert.NotNull(actualSlug);
            Assert.Equal(expectedSlug, actualSlug);
        }

        [Fact]
        public void Slug_ReturnsNullWhenPageUrlDoesNotHaveSubDirectory() {
            // Arrange
            var show = new PodcastShow {
                PageUrl = "https://test.com/"
            };

            // Act
            var actualSlug = show.Slug;

            // Assert
            Assert.Null(actualSlug);
        }
    }
}