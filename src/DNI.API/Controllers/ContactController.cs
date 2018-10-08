using System.Threading.Tasks;

using DNI.API.Requests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNI.API.Controllers {
    public class ContactController : ControllerBase {

        public ContactController() {
            
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

            // TODO: Send email

            return NoContent();

        }
    }
}