using System;
using System.Linq;
using System.Threading.Tasks;

using DNI.API.Requests;
using DNI.API.Responses;
using DNI.Services.Show;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNI.API.Controllers {
    /// <summary>
    ///     Provides REST methods for shows
    /// </summary>
    public class ShowsController : ControllerBase {
        private readonly IShowService _showService;

        /// <summary>
        ///     Provides REST methods for shows
        /// </summary>
        /// <param name="showService"></param>
        public ShowsController(IShowService showService) {
            _showService = showService;
        }

        /// <summary>
        ///     Retrieves a list of shows that have been tagged with a specific keyword from the podcast service
        /// </summary>
        /// <returns></returns>
        /// <response code="400">A validation error occurred. See raised <see cref="APIErrorResponse" /> for more details.</response>
        /// <response code="200">OK. Shows were successfully returned.</response>
        /// <response code="204">OK - No Content. No shows were found.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ShowListAPIResponse), StatusCodes.Status200OK)]
        [Route("shows/{keyword}")]
        public async Task<IActionResult> GetShowsAsync([FromQuery] GetShowsRequest request, [FromRoute] string keyword) {
            if(!ModelState.IsValid) {
                return ModelValidationBadRequest();
            }

            var showList = await _showService.GetShowsAsync(request, request, keyword);

            if(showList == null || !showList.Items.Any()) {
                return NoContent();
            }

            var keywords = await _showService.GetAggregatedKeywords();

            var response = new ShowListAPIResponse {
                PageInfo = new PagedAPIResponse {
                    StartIndex = showList.StartIndex,
                    TotalRecords = showList.TotalRecords,
                    EndIndex = showList.EndIndex,
                    CurrentPage = showList.CurrentPage,
                    TotalPages = showList.TotalPages,
                    ItemsPerPage = showList.ItemsPerPage
                },
                PagedShows = showList.Items,
                ShowKeywords = keywords
            };

            return Ok(response);
        }

        /// <summary>
        ///     Retrieves a list of all shows from the podcast service
        /// </summary>
        /// <returns></returns>
        /// <response code="400">A validation error occurred. See raised <see cref="APIErrorResponse" /> for more details.</response>
        /// <response code="200">OK. Shows were successfully returned.</response>
        /// <response code="204">OK - No Content. No shows were found.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ShowListAPIResponse), StatusCodes.Status200OK)]
        [Route("shows")]
        public async Task<IActionResult> GetShowsAsync([FromQuery] GetShowsRequest request) {
            return await GetShowsAsync(request, null);
        }

        /// <summary>
        ///     Retrieves a <see cref="Show" /> by its slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Show), StatusCodes.Status200OK)]
        [Route("show/{slug}")]
        public async Task<IActionResult> GetShowBySlugAsync([FromRoute] string slug) {
            if(!ModelState.IsValid) {
                return ModelValidationBadRequest();
            }

            Show show;
            try {
                show = await _showService.GetShowBySlugAsync(slug);
            } catch(InvalidOperationException) {
                ModelState.AddModelError(nameof(slug), "The specified slug does not exist");
                return ModelValidationBadRequest();
            }

            return Ok(show);
        }

        // TODO: Add "GetLatestShowAsync"
    }
}