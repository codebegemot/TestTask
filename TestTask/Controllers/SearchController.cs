using Microsoft.AspNetCore.Mvc;
using TestTask.Models;
using TestTask.Services;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("ping")]
        public async Task<IActionResult> Ping(CancellationToken cancellationToken)
        {
            var isAvailable = await _searchService.IsAvailableAsync(cancellationToken);
            if (isAvailable)
            {
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpPost("search")]
        public async Task<ActionResult<SearchResponse>> Search([FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            var response = await _searchService.SearchAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}

