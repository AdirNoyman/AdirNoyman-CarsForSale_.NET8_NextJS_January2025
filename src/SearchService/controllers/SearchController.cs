using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
        {
            // Create the paged search query object
            var query = DB.PagedSearch<Item, Item>();
           
            // Search the items based on the search parameter SearchTerm
            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
            }
            
            // Order (sort) the items based on the search parameter
            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(y => y.Make)),
                "new" => query.Sort(x => x.Descending(y => y.CreatedAt)),
                // The default sort order is by AuctionEnd time from the soonest to the latest                
                _ => query.Sort(x => x.Ascending(y => y.AuctionEnd))
            };

            // Filter the items based on the search parameter
            query = searchParams.FilterBy switch
            {
                // Get auctions that are already finished
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                // Get auctions that are still ongoing but will end in the next 6 hours
                "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
                // The default filter is to show all auctions that are still ongoing
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };

            // Filter the items based on the search parameter Seller
            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query.Match(x => x.Seller == searchParams.Seller);
            }

            // Filter the items based on the search parameter Winner
            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                query.Match(x => x.Winner == searchParams.Winner);
            }

            // Page the search results => Set the page number and page size
            query.PageNumber(searchParams.PageNumber).PageSize(searchParams.PageSize);
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