using System.Threading.Tasks;

using DNI.API.Responses;
using DNI.Services.ShowList;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

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
        ///     Retrieves a list of aggregated and merged shows from both podcast and vodcast services
        /// </summary>
        /// <returns></returns>
        /// <response code="400">A validation error occurred. See raised <see cref="APIErrorResponse" /> for more details.</response>
        /// <response code="200">OK. Shows were successfully returned.</response>
        /// <response code="204">OK - No Content. No shows were found.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Show[]), StatusCodes.Status200OK)]
        [Route("shows")]
        public async Task<IActionResult> GetShowsAsync([FromQuery] ShowOrderField orderByField = ShowOrderField.PublishedTime, [FromQuery] ShowOrderFieldOrder orderByOrder = ShowOrderFieldOrder.Descending) {
            if(!ModelState.IsValid) {
                return ModelValidationBadRequest();
            }

            var shows = await _showListService.GetShowsAsync(orderByField, orderByOrder);

            if(!shows.Any()) {
                return NoContent();
            }

            return Ok(shows);
        }
    }
}