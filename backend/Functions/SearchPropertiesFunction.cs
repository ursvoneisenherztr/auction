using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Auction.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Auction.API.Functions
{
    /// <summary>
    /// Azure Function to search properties by city, type, and price
    /// GET /api/properties/search?city=Zurich&type=Apartment&maxPrice=600000
    /// </summary>
    public class SearchPropertiesFunction
    {
        private readonly AuctionDbContext _context;

        public SearchPropertiesFunction(AuctionDbContext context)
        {
            _context = context;
        }

        [Function("SearchProperties")]
        public async Task<IActionResult> Run(
            [HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Anonymous, "get", Route = "properties/search")] HttpRequest req,
            FunctionContext context)
        {
            var log = context.GetLogger(nameof(SearchPropertiesFunction));
            try
            {
                var city = req.Query["city"].ToString();
                var type = req.Query["type"].ToString();
                var maxPrice = decimal.TryParse(req.Query["maxPrice"], out var price) ? price : decimal.MaxValue;
                var minPrice = decimal.TryParse(req.Query["minPrice"], out var minP) ? minP : 0;

                log.LogInformation($"Searching properties: city={city}, type={type}, price={minPrice}-{maxPrice}");

                var query = _context.Properties.Where(p => p.IsActive);

                if (!string.IsNullOrEmpty(city))
                    query = query.Where(p => p.City.ToLower().Contains(city.ToLower()));

                if (!string.IsNullOrEmpty(type))
                    query = query.Where(p => p.PropertyType.ToLower() == type.ToLower());

                query = query.Where(p => p.StartPrice >= minPrice && p.StartPrice <= maxPrice);

                var results = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    success = true,
                    data = results,
                    count = results.Count
                });
            }
            catch (Exception ex)
            {
                log.LogError($"Error: {ex.Message}");
                return new ObjectResult(new { success = false, error = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
