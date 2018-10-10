using System.Collections.Generic;
using System.Threading.Tasks;

using DNI.API.Requests;
using DNI.Options;
using DNI.Services.Email;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DNI.API.Controllers {
    public class ContactController : ControllerBase {
        private readonly IEmailService _emailService;
        private readonly GeneralOptions _generalOptions;

        public ContactController(IEmailService emailService, IOptions<GeneralOptions> generalOptions) {
            _emailService = emailService;
            _generalOptions = generalOptions.Value;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("contact")]
        public async Task<IActionResult> ContactAsync([FromBody] ContactRequest req) {
            if(!ModelState.IsValid) {
                return ModelValidationBadRequest();
            }

            var message = req.message;
            message = message.Replace("\r\n", "<br/>");
            message = message.Replace("\n", "<br/>");
            message = message.Replace("\r", "<br/>");

            await _emailService.CompileAndSendAsync(new CompileAndSendEmailRequest {
                From = _generalOptions.ErrorEmailFrom,
                To = _generalOptions.ContactEmailTo,
                ReplyTo = req.email,
                EmailTemplateResourceId = _generalOptions.ContactTemplateResourceId,
                HtmlLayoutResourceId = _generalOptions.HtmlEmailLayoutResourceId,
                TemplateReplacements = new Dictionary<string, string> {
                    {"name", req.name},
                    {"message", message},
                    {"marketingOptOut", req.marketingOptOut ? "Yes" : "No"},
                    {"deleteDetails", req.deleteDetails ? "Yes" : "No"}
                },
                LayoutReplacements = new Dictionary<string, string> {
                    {"logoUrl", _generalOptions.GetLogoUrl()}
                }
            });

            return NoContent();
        }
    }
}