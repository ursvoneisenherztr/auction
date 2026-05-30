using Microsoft.EntityFrameworkCore;
using Auction.API.Models;

namespace Auction.API.Data
{
    /// <summary>
    /// Database context for Auction application using SQL Server
    /// </summary>
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Models.Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Property Configuration
            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.PropertyId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StartPrice).HasPrecision(18, 2);
                entity.Property(e => e.EstimatedValue).HasPrecision(18, 2);
                entity.Property(e => e.SquareMeters).HasPrecision(10, 2);
                entity.Property(e => e.LandArea).HasPrecision(10, 2);
                entity.Property(e => e.Latitude).HasPrecision(10, 8);
                entity.Property(e => e.Longitude).HasPrecision(11, 8);
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.PropertyType);
            });

            // Auction Configuration
            modelBuilder.Entity<Models.Auction>(entity =>
            {
                entity.HasKey(e => e.AuctionId);
                entity.Property(e => e.AuctionNumber).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.AuctionNumber).IsUnique();
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.MinimumBid).HasPrecision(18, 2);
                entity.Property(e => e.CurrentHighestBid).HasPrecision(18, 2);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.StartDate, e.EndDate });

                // Foreign keys
                entity.HasOne(e => e.Property)
                    .WithOne(p => p.Auction)
                    .HasForeignKey<Models.Auction>(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CurrentHighestBidder)
                    .WithMany()
                    .HasForeignKey(e => e.CurrentHighestBidderId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Bid Configuration
            modelBuilder.Entity<Bid>(entity =>
            {
                entity.HasKey(e => e.BidId);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasIndex(e => e.AuctionId);
                entity.HasIndex(e => e.BidderId);

                // Foreign keys
                entity.HasOne(e => e.Auction)
                    .WithMany(a => a.Bids)
                    .HasForeignKey(e => e.AuctionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Bidder)
                    .WithMany(u => u.Bids)
                    .HasForeignKey(e => e.BidderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PropertyImage Configuration
            modelBuilder.Entity<PropertyImage>(entity =>
            {
                entity.HasKey(e => e.ImageId);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);

                // Foreign key
                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Images)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Favorite Configuration
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => e.FavoriteId);
                entity.HasIndex(e => new { e.UserId, e.PropertyId }).IsUnique();

                // Foreign keys
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId);
                entity.Property(e => e.Type).HasMaxLength(50);

                // Foreign keys
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Property)
                    .WithMany()
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Auction)
                    .WithMany()
                    .HasForeignKey(e => e.AuctionId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
