using System;
using System.Collections.Generic;

namespace Auction.API.Models
{
    /// <summary>
    /// User/Bidder Model
    /// </summary>
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    /// <summary>
    /// Property Model - represents a real estate property in auction
    /// </summary>
    public class Property
    {
        public Guid PropertyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        
        // Property Details
        public string PropertyType { get; set; } // House, Apartment, Commercial, Land
        public decimal? SquareMeters { get; set; }
        public decimal? Rooms { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? YearBuilt { get; set; }
        public string Condition { get; set; } // Good, Fair, Poor, Renovation
        public string RoofType { get; set; }
        public string HeatingType { get; set; }
        public bool HasGarage { get; set; }
        public int GarageSpaces { get; set; }
        public decimal? LandArea { get; set; }
        
        // Pricing
        public decimal StartPrice { get; set; }
        public decimal? EstimatedValue { get; set; }
        
        // Location
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        
        // Media
        public string ImageUrl { get; set; }
        public int ImagesTotalCount { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public Auction Auction { get; set; }
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }

    /// <summary>
    /// Auction Model - represents an active auction for a property
    /// </summary>
    public class Auction
    {
        public Guid AuctionId { get; set; }
        public Guid PropertyId { get; set; }
        public string AuctionNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } // Scheduled, Active, Ended, Cancelled
        public decimal? MinimumBid { get; set; }
        public decimal CurrentHighestBid { get; set; }
        public Guid? CurrentHighestBidderId { get; set; }
        public int BidCount { get; set; }
        
        // Seller & Court Info
        public string SellerName { get; set; }
        public string SellerInfo { get; set; }
        public string CourtName { get; set; }
        public string JudgeName { get; set; }
        
        // Details
        public string AuctionType { get; set; } // Forced Sale, Bankruptcy, Regular
        public string Reason { get; set; }
        public string TermsAndConditions { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Property Property { get; set; }
        public User CurrentHighestBidder { get; set; }
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    }

    /// <summary>
    /// Bid Model - represents a bid placed on an auction
    /// </summary>
    public class Bid
    {
        public Guid BidId { get; set; }
        public Guid AuctionId { get; set; }
        public Guid BidderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
        public bool IsWinningBid { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Auction Auction { get; set; }
        public User Bidder { get; set; }
    }

    /// <summary>
    /// PropertyImage Model - additional images for a property
    /// </summary>
    public class PropertyImage
    {
        public Guid ImageId { get; set; }
        public Guid PropertyId { get; set; }
        public string ImageUrl { get; set; }
        public string Caption { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Property Property { get; set; }
    }

    /// <summary>
    /// Favorite Model - user's favorite properties
    /// </summary>
    public class Favorite
    {
        public Guid FavoriteId { get; set; }
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Property Property { get; set; }
    }

    /// <summary>
    /// Notification Model - user notifications
    /// </summary>
    public class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } // AuctionEnding, OutbidNotice, AuctionStarted
        public string Title { get; set; }
        public string Message { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? AuctionId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Property Property { get; set; }
        public Auction Auction { get; set; }
    }
}
