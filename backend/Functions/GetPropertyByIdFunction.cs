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
    /// Azure Function to retrieve a specific property by ID
    /// GET /api/properties/{id}
    /// </summary>
    public class GetPropertyByIdFunction
    {
        private readonly AuctionDbContext _context;

        public GetPropertyByIdFunction(AuctionDbContext context)
        {
            _context = context;
        }

        [Function("GetPropertyById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Anonymous, "get", Route = "properties/{id}")] HttpRequest req,
            string id,
            FunctionContext context)
        {
            var log = context.GetLogger(nameof(GetPropertyByIdFunction));
            try
            {
                if (!Guid.TryParse(id, out var propertyId))
                {
                    return new BadRequestObjectResult(new { success = false, error = "Invalid property ID format" });
                }

                log.LogInformation($"Fetching property: {propertyId}");

                var property = await _context.Properties
                    .Include(p => p.Auction)
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.PropertyId == propertyId);

                if (property == null)
                {
                    return new NotFoundObjectResult(new { success = false, error = "Property not found" });
                }

                return new OkObjectResult(new { success = true, data = property });
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
