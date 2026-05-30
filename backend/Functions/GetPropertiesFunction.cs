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
    /// Azure Function to retrieve all active properties with pagination
    /// GET /api/properties?skip=0&take=10
    /// </summary>
    public class GetPropertiesFunction
    {
        private readonly AuctionDbContext _context;

        public GetPropertiesFunction(AuctionDbContext context)
        {
            _context = context;
        }

        [Function("GetProperties")]
        public async Task<IActionResult> Run(
            [HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Anonymous, "get", Route = "properties")] HttpRequest req,
            FunctionContext context)
        {
            try
            {
                var log = context.GetLogger(nameof(GetPropertiesFunction));
                log.LogInformation("Fetching all properties");

                var skip = int.TryParse(req.Query["skip"], out var s) ? s : 0;
                var take = int.TryParse(req.Query["take"], out var t) ? Math.Min(t, 100) : 10;

                var properties = await _context.Properties
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                var total = await _context.Properties.CountAsync(p => p.IsActive);

                return new OkObjectResult(new
                {
                    success = true,
                    data = properties,
                    pagination = new { skip, take, total, hasMore = skip + take < total }
                });
            }
            catch (Exception ex)
            {
                var log = context.GetLogger(nameof(GetPropertiesFunction));
                log.LogError($"Error: {ex.Message}");
                return new ObjectResult(new { success = false, error = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
