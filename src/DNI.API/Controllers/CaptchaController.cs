using System.Threading.Tasks;

using DNI.API.Requests;
using DNI.API.Responses;
using DNI.Services.Captcha;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNI.API.Controllers {
    /// <summary>
    ///     Provides REST methods for CAPTCHA
    /// </summary>
    public class CaptchaController : ControllerBase {
        private readonly ICaptchaService _captchaService;

        /// <summary>
        ///     Provides REST methods for CAPTCHA
        /// </summary>
        /// <param name="captchaService"></param>
        public CaptchaController(ICaptchaService captchaService) {
            _captchaService = captchaService;
        }

        /// <summary>
        ///     Used for validating a Google ReCaptcha user response.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <response code="400">A validation error occurred. See raised <see cref="APIErrorResponse" /> for more details.</response>
        /// <response code="204">No Content. The CAPTCHA was successfully validated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("captcha")]
        public async Task<IActionResult> CaptchaAsync([FromBody] CaptchaRequest req) {
            if(!ModelState.IsValid) {
                return ModelValidationBadRequest();
            }

            var ip = Request.GetClientIp();
            var verified = await _captchaService.VerifyAsync(req.UserResponse, ip);
            if(!verified) {
                ModelState.AddModelError(nameof(req.UserResponse), "The CAPTCHA could not be verified");
                return ModelValidationBadRequest();
            }

            return NoContent();
        }
    }
}