using System.Xml.Linq;
using System.Xml.XPath;

namespace DNI.Services.Podcast {
    /// <summary>
    ///     Provides helper extension methods for working with XML documents
    /// </summary>
    public static class XDocumentExtensions {
        /// <summary>
        ///     Returns the specified element value as a string
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string GetElementValue(this XDocument doc, string elementName) {
            return doc.GetElement(elementName)?.Value;
        }

        /// <summary>
        ///     Returns the specified <see cref="XElement" />
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static XElement GetElement(this XDocument doc, string elementName) {
            return doc.XPathSelectElement($"//*[name()='{elementName}']");
        }

        /// <summary>
        ///     Returns the specified element's attribute value.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementName"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string GetAttributeValue(this XDocument doc, string elementName, string attributeName) {
            return doc.GetElement(elementName)?.Attribute(attributeName)?.Value;
        }
    }
}