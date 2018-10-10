using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DNI.Services.Email {
    /// <summary>
    ///     Provides a base email class with simple HTML templating features.
    /// </summary>
    public abstract class EmailTemplateServiceBase : IEmailService {
        /// <summary>
        ///     Sends an email asynchronously. Override to customise send behaviour.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public abstract Task<bool> SendAsync(SendEmailRequest req);

        /// <summary>
        ///     Validates the email inputs.
        /// </summary>
        /// <param name="req"></param>
        protected void ValidateRequest(SendEmailRequest req) {
            if(req == null) throw new ArgumentNullException(nameof(req));
            if(string.IsNullOrWhiteSpace(req.To)) throw new ArgumentNullException(nameof(req.To));
            if(string.IsNullOrWhiteSpace(req.From)) throw new ArgumentNullException(nameof(req.From));
            if(string.IsNullOrWhiteSpace(req.Subject)) throw new ArgumentNullException(nameof(req.Subject));
            if(string.IsNullOrWhiteSpace(req.Body)) throw new ArgumentNullException(nameof(req.Body));
        }

        /// <summary>
        ///     Compiles a HTML template and sends an email
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<bool> CompileAndSendAsync(CompileAndSendEmailRequest req) {
            // Read html layout as a string
            var htmlBody = ReadLayoutResource(req.ResourceAssembly, req.HtmlLayoutResourceId);

            // Load and compile the template
            var template = ReadTemplateResource(req.ResourceAssembly, req.EmailTemplateResourceId);
            template = ReplacePlaceholders(template, req.TemplateReplacements);

            // Load the template into the layout
            htmlBody = CompileHtmlBody(htmlBody, template, req.LayoutReplacements);

            // Send the email and return the result
            return await SendAsync(new SendEmailRequest {
                To = req.To,
                From = req.From ?? template.from,
                Body = template.plainTextBody,
                HtmlBody = htmlBody,
                Subject = template.subject,
                ReplyTo = req.ReplyTo
            });
        }

        /// <summary>
        ///     Reads the template specified in <see cref="resourceName" />. The template should be marked as an "Embedded
        ///     Resource" and exist in the specified <see cref="resourceAssembly" />
        /// </summary>
        /// <param name="resourceAssembly"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public EmailTemplate ReadTemplateResource(Assembly resourceAssembly, string resourceName) {
            var plain = ReadLayoutResource(resourceAssembly, resourceName);
            return JsonConvert.DeserializeObject<EmailTemplate>(plain);
        }

        /// <summary>
        ///     Reads the layout template specified in <see cref="resourceName" />. The template should be marked as an "Embedded
        ///     Resource" and exist in the specified <see cref="resourceAssembly" />
        /// </summary>
        /// <param name="resourceAssembly"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public string ReadLayoutResource(Assembly resourceAssembly, string resourceName) {
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

        /// <summary>
        ///     Replaces {{tokens}} specified in the fields inside <paramref name="template" /> parameter with the replacement
        ///     strings
        ///     specified in <paramref name="replacements" />
        /// </summary>
        /// <param name="template"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public EmailTemplate ReplacePlaceholders(EmailTemplate template, IDictionary<string, string> replacements) {
            var members = GetPublicStrings(template);

            // Loop over the string properties first, as it's more efficient
            foreach(var prop in members) {
                if(!(prop.GetValue(template) is string propValue)) {
                    continue;
                }

                // Replace all placeholders in this member's value
                foreach(var placeholder in replacements) {
                    var p = string.Concat("{{", placeholder.Key, "}}");
                    propValue = propValue.Replace(p, placeholder.Value);
                }

                prop.SetValue(template, propValue);
            }

            return template;
        }

        /// <summary>
        ///     Takes a HTML string (<paramref name="layoutHtml" />) and applies the member values in <paramref name="template" />
        ///     to it.
        /// </summary>
        /// <param name="layoutHtml"></param>
        /// <param name="template"></param>
        /// <param name="additionalLayoutReplacements"></param>
        /// <returns></returns>
        public string CompileHtmlBody(string layoutHtml, EmailTemplate template, IDictionary<string, string> additionalLayoutReplacements) {
            var members = GetPublicStrings(template);

            foreach(var prop in members) {
                if(!(prop.GetValue(template) is string propValue)) {
                    continue;
                }

                // Replace member value in the layout template
                var p = string.Concat("{{", prop.Name, "}}");
                layoutHtml = layoutHtml.Replace(p, propValue);
            }

            // Perform any additional replacements specified for the HTML Layout.
            // Note that this is not the same as the template replacements, and 
            // is specifically used to replace HTML layout placeholders 
            // such as {{logoUrl}}
            if(additionalLayoutReplacements != null) {
                foreach(var placeholder in additionalLayoutReplacements) {
                    var p = string.Concat("{{", placeholder.Key, "}}");
                    layoutHtml = layoutHtml.Replace(p, placeholder.Value);
                }
            }

            return layoutHtml;
        }

        private static IEnumerable<PropertyInfo> GetPublicStrings(object template) {
            return template
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.PropertyType == typeof(string));
        }
    }
}