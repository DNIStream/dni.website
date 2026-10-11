import sanitizeHtml from "sanitize-html";

const OPTIONS = {
  allowedTags: ["p", "ul", "ol", "li", "h2", "h3", "h4", "strong", "em", "b", "i", "a", "img", "br", "blockquote", "code", "pre"],
  allowedAttributes: { a: ["href", "rel"], img: ["src", "alt", "width", "height"] },
  allowedSchemes: ["http", "https", "mailto"],
  allowedSchemesByTag: { img: ["https"] },
  disallowedTagsMode: "discard",
  nonTextTags: ["script", "style", "iframe", "noscript", "textarea", "option"],
  transformTags: {
    a: (tagName, attribs) => {
      const external = /^https?:/i.test(attribs.href || "");
      return { tagName, attribs: external ? { ...attribs, rel: "noopener noreferrer" } : attribs };
    },
  },
};

export function sanitizeNotes(html) {
  if (!html) return "";
  return sanitizeHtml(String(html), OPTIONS).trim();
}
