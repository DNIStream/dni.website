using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace DNI.Services.Email {
    public interface IEmailService {
        Task<bool> SendAsync(SendEmailRequest req);

        Task<bool> CompileAndSendAsync(CompileAndSendEmailRequest req);

        EmailTemplate ReadTemplateResource(Assembly resourceAssembly, string resourceName);

        string ReadLayoutResource(Assembly resourceAssembly, string resourceName);

        EmailTemplate ReplacePlaceholders(EmailTemplate template, IDictionary<string, string> replacements);

        string CompileHtmlBody(string layoutHtml, EmailTemplate template, IDictionary<string, string> additionalLayoutReplacements);
    }
}