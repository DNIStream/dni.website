using System.Linq;
using System.Threading.Tasks;

using DNI.API.Requests;
using DNI.API.Responses;
using DNI.Services.ShowList;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNI.API.Controllers {
    /// <summary>
    ///     Provides REST methods for shows
    /// </summary>
    public class ShowsController : ControllerBase {
        private readonly IShowListService _showListService;

        /// <summary>
        ///     Provides REST methods for shows
        /// </summary>
        /// <param name="showListService"></param>
        public ShowsController(IShowListService showListService) {
            _showListService = showListService;
        }

        /// <summary>
        ///     Retrieves a list shows from the podcast service
        /// </summary>
        /// <returns></returns>
        /// <response code="400">A validation error occurred. See raised <see cref="APIErrorResponse" /> for more details.</response>
        /// <response code="200">OK. Shows were successfully returned.</response>
        /// <response code="204">OK - No Content. No shows were found.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ShowList), StatusCodes.Status200OK)]
        [Route("shows")]
        public async Task<IActionResult> GetShowsAsync([FromBody] GetShowsRequest request) {
            if(!ModelState.IsValid) {
                return ModelValidationBadRequest();
            }

            var showList = await _showListService.GetShowListAsync(request, request);

            if(!showList.Items.Any()) {
                return NoContent();
            }

            return Ok(showList);
        }
    }
}