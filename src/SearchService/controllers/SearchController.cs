using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems(string searchTerm, int pageNumber = 1, int pageSize = 4)
        {
            var query = DB.PagedSearch<Item>();

            query.Sort(x => x.Ascending(y => y.Make));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query.Match(Search.Full, searchTerm).SortByTextScore();
            }

            query.PageNumber(pageNumber).PageSize(pageSize);
            var result = await query.ExecuteAsync();

            return Ok(new
            {

                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount

            });

        }

    }
}