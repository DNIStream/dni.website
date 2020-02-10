using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Services.Podcast;
using DNI.Testing;

using RestSharp;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests.Podcast {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class FiresideRssDeserializerUnitTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        private readonly string RssResourceFile = "DNI.Services.Tests.TestResources.FiresideRss.xml";

        public FiresideRssDeserializerUnitTests(ITestOutputHelper output) {
            _output = output;
        }

        private FiresideRssDeserializer GetDeserializer() {
            return new FiresideRssDeserializer();
        }

        private string GetFiresideRss() {
            return ReadResource(Assembly.GetExecutingAssembly(), RssResourceFile);
        }

        private IRestResponse GetMockResponse() {
            return _fixture.Build<RestResponse>()
                .With(x => x.StatusCode, HttpStatusCode.Accepted)
                .With(x => x.ContentType, "application/xml")
                .With(x => x.Content, GetFiresideRss())
                .Create();
        }

        [Fact]
        public void Deserialize_ReturnsPodcastStreamParent() {
            // Arrange
            var service = GetDeserializer();
            var response = GetMockResponse();

            // Act
            var objectGraph = service.Deserialize<PodcastStream>(response);

            // Assert
            Assert.NotNull(objectGraph);
        }

        [Fact]
        public void Deserialize_ReturnsSamesNumberOfShowsAsInInputRss() {
            // Arrange
            var service = GetDeserializer();
            var response = GetMockResponse();

            // Act
            var objectGraph = service.Deserialize<PodcastStream>(response);

            // Assert
            // Input RSS has 3 items in it
            Assert.Equal(3, objectGraph.Shows.Count);
        }

        [Fact]
        public void Deserialize_RssItemPropertiesAreMappedToShowMembers() {
            // Arrange
            var service = GetDeserializer();
            var response = GetMockResponse();

            // Act
            var objectGraph = service.Deserialize<PodcastStream>(response);
            var itemUnderTest = objectGraph.Shows[0];

            // Assert
            Assert.Equal("How I Learned To Stay Relevant and Love the Code", itemUnderTest.Title);
            Assert.Equal("TEST\nPLAIN TEXT\nContent\nJEFF\n", itemUnderTest.Content);
            Assert.Equal("<p>This is some test HTML</p>\n<p>And another line</p>", itemUnderTest.ContentHtml);
            Assert.Equal("This is the expected subtitle", itemUnderTest.Summary);
            // "Fri, 24 Jan 2020 05:15:00 +0000" in file
            Assert.Equal(new DateTime(2020, 1, 24, 5, 15, 0), itemUnderTest.DatePublished);
            Assert.Equal(new Guid("9fba4db0-ff9b-4364-baa6-0d14eabc1ab7"), itemUnderTest.Id);
            Assert.Equal("https://dnistream.fireside.fm/49", itemUnderTest.PageUrl);
            // <itunes:image href="https://assets.fireside.fm/myimage.jpg"/>
            Assert.Equal("https://assets.fireside.fm/myimage.jpg", itemUnderTest.HeaderImage);
        }

        [Fact]
        public void Deserialize_RssEnclosureMapsToShowAudioFile() {
            // Arrange
            var service = GetDeserializer();
            var response = GetMockResponse();

            // Act
            var objectGraph = service.Deserialize<PodcastStream>(response);
            var itemUnderTest = objectGraph.Shows[0];

            // Assert
            Assert.NotNull(itemUnderTest.AudioFile);
            // <enclosure url="https://test.com/mymp3.mp3" length="48168849" type="audio/mpeg"/>
            Assert.Equal("https://test.com/mymp3.mp3", itemUnderTest.AudioFile.Url);
            Assert.Equal("1:05:52", itemUnderTest.AudioFile.Duration);
            Assert.Equal("audio/mpeg", itemUnderTest.AudioFile.MimeType);
            Assert.Equal(48168849, itemUnderTest.AudioFile.SizeInBytes);
        }

        [Theory]
        [InlineData(" dev, dev, test,antelope")]
        [InlineData("dev, dev, test,antelope")]
        [InlineData(" dev, DEV, test,antelope")]
        [InlineData("Dev , dev, test,antelope")]
        [InlineData("dev , ,dev, test,antelope")]
        [InlineData("dev ,,dev, test,antelope")]
        public void Deserialize_RssKeywordsAreDeserialized_And_DuplicateRssKeywordsAreDistinct(string keywords) {
            // Arrange
            var service = GetDeserializer();
            var response = GetMockResponse();
            response.Content = response.Content
                .Replace("REPLACE IN TEST", keywords);

            // Act
            var objectGraph = service.Deserialize<PodcastStream>(response);

            // Assert
            var itemUnderTest = objectGraph.Shows[0];
            Assert.Equal(3, itemUnderTest.Keywords.Count());
            Assert.Contains("dev", itemUnderTest.Keywords);
            Assert.Contains("test", itemUnderTest.Keywords);
            Assert.Contains("antelope", itemUnderTest.Keywords);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r\n")]
        [InlineData("\n")]
        [InlineData("\t\n")]
        [InlineData("\t\n    ")]
        [InlineData(null)]
        public void Deserialize_NullOrEmptyOrWhitespaceRssKeywords_ReturnNull(string keywords) {
            // Arrange
            var service = GetDeserializer();
            var response = GetMockResponse();
            response.Content = response.Content
                .Replace("REPLACE IN TEST", keywords);

            // Act
            var objectGraph = service.Deserialize<PodcastStream>(response);

            // Assert
            var itemUnderTest = objectGraph.Shows[0];
            Assert.Null(itemUnderTest.Keywords);
        }

        #region Helper Methods

        public string ReadResource(Assembly resourceAssembly, string resourceName) {
            if(resourceAssembly == null) {
                resourceAssembly = Assembly.GetExecutingAssembly();
            }

            using(var stream = resourceAssembly.GetManifestResourceStream(resourceName)) {
                if(stream == null) {
                    throw new InvalidOperationException($"Unable to find resource '{resourceName}'");
                }

                using(var reader = new StreamReader(stream)) {
                    var content = reader.ReadToEnd();

                    return content;
                }
            }
        }

        #endregion
    }
}