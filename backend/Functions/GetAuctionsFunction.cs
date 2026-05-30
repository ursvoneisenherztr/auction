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
    /// Azure Function to retrieve all auctions with their properties
    /// GET /api/auctions?status=Active
    /// </summary>
    public class GetAuctionsFunction
    {
        private readonly AuctionDbContext _context;

        public GetAuctionsFunction(AuctionDbContext context)
        {
            _context = context;
        }

        [Function("GetAuctions")]
        public async Task<IActionResult> Run(
            [HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Anonymous, "get", Route = "auctions")] HttpRequest req,
            FunctionContext context)
        {
            var log = context.GetLogger(nameof(GetAuctionsFunction));
            try
            {
                var status = req.Query["status"].ToString();
                log.LogInformation($"Fetching auctions with status: {status}");

                var query = _context.Auctions.Include(a => a.Property).AsQueryable();

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(a => a.Status == status);

                var auctions = await query
                    .OrderByDescending(a => a.StartDate)
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    success = true,
                    data = auctions,
                    count = auctions.Count
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
