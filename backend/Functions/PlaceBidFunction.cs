using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Auction.API.Data;
using Auction.API.Models;

namespace Auction.API.Functions
{
    /// <summary>
    /// Azure Function to place a bid on an auction
    /// POST /api/auctions/{auctionId}/bid
    /// Body: { "bidderId": "guid", "amount": 500000 }
    /// </summary>
    public class PlaceBidFunction
    {
        private readonly AuctionDbContext _context;

        public PlaceBidFunction(AuctionDbContext context)
        {
            _context = context;
        }

        [Function("PlaceBid")]
        public async Task<IActionResult> Run(
            [HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Anonymous, "post", Route = "auctions/{auctionId}/bid")] HttpRequest req,
            string auctionId,
            FunctionContext context)
        {
            var log = context.GetLogger(nameof(PlaceBidFunction));
            try
            {
                if (!Guid.TryParse(auctionId, out var auctionGuid))
                {
                    return new BadRequestObjectResult(new { success = false, error = "Invalid auction ID" });
                }

                var requestBody = await JsonSerializer.DeserializeAsync<PlaceBidRequest>(req.Body);
                if (requestBody?.Amount <= 0)
                {
                    return new BadRequestObjectResult(new { success = false, error = "Invalid bid amount" });
                }

                log.LogInformation($"Placing bid on auction {auctionId}: ${requestBody.Amount}");

                var auction = await _context.Auctions.FindAsync(auctionGuid);
                if (auction == null)
                {
                    return new NotFoundObjectResult(new { success = false, error = "Auction not found" });
                }

                if (auction.Status != "Active")
                {
                    return new BadRequestObjectResult(new { success = false, error = "Auction is not active" });
                }

                if (requestBody.Amount <= auction.CurrentHighestBid)
                {
                    return new BadRequestObjectResult(new 
                    { 
                        success = false, 
                        error = $"Bid must be higher than {auction.CurrentHighestBid}" 
                    });
                }

                var bid = new Bid
                {
                    BidId = Guid.NewGuid(),
                    AuctionId = auctionGuid,
                    BidderId = requestBody.BidderId,
                    Amount = requestBody.Amount,
                    BidTime = DateTime.UtcNow,
                    IsWinningBid = true
                };

                auction.CurrentHighestBid = requestBody.Amount;
                auction.CurrentHighestBidderId = requestBody.BidderId;
                auction.BidCount++;

                _context.Bids.Add(bid);
                await _context.SaveChangesAsync();

                return new CreatedResult($"/auctions/{auctionId}/bid/{bid.BidId}", new
                {
                    success = true,
                    data = bid,
                    message = "Bid placed successfully"
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

    public class PlaceBidRequest
    {
        public Guid BidderId { get; set; }
        public decimal Amount { get; set; }
    }
}
