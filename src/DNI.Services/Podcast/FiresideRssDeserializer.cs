using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;

using RestSharp;
using RestSharp.Serialization.Xml;

namespace DNI.Services.Podcast {
    /// <summary>
    ///     Deserializes a Fireside RSS feed into a <see cref="PodcastShow" />
    /// </summary>
    public class FiresideRssDeserializer : IXmlDeserializer {
        /// <summary>
        ///     Sanitises RSS iTunes keywords
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        private IEnumerable<string> SanitiseKeyWords(string keywords) {
            return keywords
                .Split(',')
                .Select(x => x.Trim().ToLower())
                .Where(x => x.Length > 0)
                .Distinct();
        }

        /// <summary>
        ///     Deserializes a Fireside RSS feed into a <see cref="PodcastShow" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public T Deserialize<T>(IRestResponse response) {
            var streamGraph = new PodcastStream();
            var rssStream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));

            // Read RSS feed into XML
            using(var xmlReader = XmlReader.Create(rssStream, new XmlReaderSettings())) {
                var feedReader = new RssFeedReader(xmlReader);
                while(feedReader.Read().GetAwaiter().GetResult()) {
                    if(feedReader.ElementType != SyndicationElementType.Item) {
                        continue;
                    }

                    var itemXml = feedReader.ReadElementAsString().GetAwaiter().GetResult();
                    var doc = XDocument.Parse(itemXml);
                    var keywords = doc.GetElementValue("itunes:keywords");

                    streamGraph.Shows.Add(new PodcastShow {
                        Id = new Guid(doc.GetElementValue("guid")),
                        Title = doc.GetElementValue("title"),
                        Content = doc.GetElementValue("description"),
                        ContentHtml = doc.GetElementValue("content:encoded"),
                        Summary = doc.GetElementValue("itunes:subtitle"),
                        PublishedTime = DateTime.TryParse(doc.GetElementValue("pubDate"), out var datePublished) ? datePublished : DateTime.MinValue,
                        PageUrl = doc.GetElementValue("link"),
                        HeaderImage = doc.GetAttributeValue("itunes:image", "href"),
                        Keywords = !string.IsNullOrWhiteSpace(keywords) ? SanitiseKeyWords(keywords) : null,
                        AudioFile = new PodcastFile {
                            Url = doc.GetAttributeValue("enclosure", "url"),
                            Duration = doc.GetElementValue("itunes:duration"),
                            MimeType = doc.GetAttributeValue("enclosure", "type"),
                            SizeInBytes = int.TryParse(doc.GetAttributeValue("enclosure", "length"), out var sizeInBytes) ? sizeInBytes : 0
                        }
                    });
                }
            }

            return (T) Convert.ChangeType(streamGraph, typeof(T));
        }

        public string RootElement { get; set; }

        public string Namespace { get; set; }

        public string DateFormat { get; set; }
    }
}