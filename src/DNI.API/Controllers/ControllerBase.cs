using System.Linq;

using DNI.API.Responses;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNI.API.Controllers {
    /// <summary>
    ///     A base class that contains some helper methods
    /// </summary>
    [Produces("application/json")]
    [ProducesResponseType(typeof(APIErrorResponse), StatusCodes.Status400BadRequest)]
    public abstract class ControllerBase : Controller {
        /// <summary>
        ///     Returns a <see cref="BadRequestObjectResult" /> containing validation errors from the ModelState.
        /// </summary>
        [NonAction]
        public BadRequestObjectResult ModelValidationBadRequest() {
            return BadRequest(new APIErrorResponse {
                Message = "One or more validation errors occurred",
                ValidationErrors = ModelState.FlattenErrors().ToArray(),
                StatusCode = StatusCodes.Status400BadRequest
            });
        }
    }
}