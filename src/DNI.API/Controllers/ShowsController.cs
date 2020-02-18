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
        ///     Retrieves a list shows from the podcast service
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
            // TODO: Implement tests
            if(!ModelState.IsValid) {
                return ModelValidationBadRequest();
            }

            var showList = await _showService.GetShowsAsync(request, request);

            if(!showList.Items.Any()) {
                return NoContent();
            }

            var latestShow = await _showService.GetLatestShowAsync();

            var keywords = await _showService.GetAggregatedKeywords();

            var test = new ShowListAPIResponse {
                PageInfo = new PagedAPIResponse {
                    StartIndex = showList.StartIndex,
                    TotalRecords = showList.TotalRecords,
                    EndIndex = showList.EndIndex,
                    CurrentPage = showList.CurrentPage,
                    TotalPages = showList.TotalPages,
                    ItemsPerPage = showList.ItemsPerPage
                },
                PagedShows = showList.Items, 
                LatestShow = latestShow,
                ShowKeywords = keywords
            };

            return Ok(test);
        }
    }
}