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
        [InlineData("https://test.com/v0-1", 0.1)]
        [InlineData("https://test.com/v13-2", 13.2)]
        [InlineData("http://test.co.uk/shows/v3-56", 3.56)]
        [InlineData("http://test.co.uk/shows/dni/v456-2223", 456.2223)]
        [InlineData("http://test.co.uk/shows/dni/V2-4", 2.4)]
        public void Version_ReturnsVersionFromVersionFormattedPageUrl(string inputUrl, decimal expectedVersion) {
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

        [Fact]
        public void DurationInSeconds_ReturnsNullWhenAudioFileIsNull() {
            // Arrange
            var show = new PodcastShow {
                AudioFile = null
            };

            // Act
            var actualDuration = show.DurationInSeconds;

            // Assert
            Assert.Null(actualDuration);
        }

        [Fact]
        public void DurationInSeconds_ReturnsNullWhenAudioFileDurationIsNull() {
            // Arrange
            var show = new PodcastShow {
                AudioFile = new PodcastFile {
                    Duration = null
                }
            };

            // Act
            var actualDuration = show.DurationInSeconds;

            // Assert
            Assert.Null(actualDuration);
        }

        [Fact]
        public void DurationInSeconds_ReturnsNullWhenAudioFileDurationCannotBeParsed() {
            // Arrange
            var show = new PodcastShow {
                AudioFile = new PodcastFile {
                    Duration = "NOT A TIME"
                }
            };

            // Act
            var actualDuration = show.DurationInSeconds;

            // Assert
            Assert.Null(actualDuration);
        }

        [Theory]
        [InlineData("02:45:43", 9943)] // ((60 x 2) x 60) + (45 x 60) + 43 = 9943
        [InlineData("45:43", 2743)] // (45 x 60) + 43 = 2743
        [InlineData("43", 43)]
        public void DurationInSeconds_ReturnsExpectedDurationInSecondsWhenAudioFileIsValidTimeStamp(string inputTimeCode, int expectedSeconds) {
            // Arrange
            var show = new PodcastShow {
                AudioFile = new PodcastFile {
                    Duration = inputTimeCode
                }
            };

            // Act
            var actualDuration = show.DurationInSeconds;

            // Assert
            Assert.Equal(expectedSeconds, actualDuration);
        }
    }
}