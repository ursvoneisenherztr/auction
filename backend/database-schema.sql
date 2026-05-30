-- Auction Database Schema for SQL Server
-- Property Auction/Forced Sale System

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'AuctionDB')
CREATE DATABASE AuctionDB;
GO

USE AuctionDB;
GO

-- Create Users/Bidders Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
CREATE TABLE Users (
    UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    PostalCode NVARCHAR(20),
    Country NVARCHAR(100) DEFAULT 'Switzerland',
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    IsActive BIT DEFAULT 1
);
GO

-- Create Properties Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Properties')
CREATE TABLE Properties (
    PropertyId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Address NVARCHAR(500) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20),
    Region NVARCHAR(100),
    Country NVARCHAR(100) DEFAULT 'Switzerland',
    PropertyType NVARCHAR(50), -- House, Apartment, Commercial, Land, etc.
    SquareMeters DECIMAL(10, 2),
    Rooms INT,
    Bedrooms INT,
    Bathrooms INT,
    YearBuilt INT,
    Condition NVARCHAR(50), -- Good, Fair, Poor, Renovation
    RoofType NVARCHAR(100),
    HeatingType NVARCHAR(100),
    HasGarage BIT DEFAULT 0,
    GarageSpaces INT DEFAULT 0,
    LandArea DECIMAL(10, 2),
    StartPrice DECIMAL(18, 2),
    EstimatedValue DECIMAL(18, 2),
    Latitude DECIMAL(10, 8),
    Longitude DECIMAL(11, 8),
    ImageUrl NVARCHAR(500),
    ImagesTotalCount INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    IsActive BIT DEFAULT 1
);
GO

-- Create Auctions Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Auctions')
CREATE TABLE Auctions (
    AuctionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PropertyId UNIQUEIDENTIFIER NOT NULL,
    AuctionNumber NVARCHAR(50) UNIQUE NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Scheduled', -- Scheduled, Active, Ended, Cancelled
    MinimumBid DECIMAL(18, 2),
    CurrentHighestBid DECIMAL(18, 2) DEFAULT 0,
    CurrentHighestBidderId UNIQUEIDENTIFIER,
    BidCount INT DEFAULT 0,
    SellerName NVARCHAR(255),
    SellerInfo NVARCHAR(MAX),
    CourtName NVARCHAR(255),
    JudgeName NVARCHAR(255),
    AuctionType NVARCHAR(50), -- Forced Sale, Bankruptcy, Regular
    Reason NVARCHAR(MAX),
    TermsAndConditions NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
    FOREIGN KEY (CurrentHighestBidderId) REFERENCES Users(UserId)
);
GO

-- Create Bids Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Bids')
CREATE TABLE Bids (
    BidId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AuctionId UNIQUEIDENTIFIER NOT NULL,
    BidderId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    BidTime DATETIME2 DEFAULT GETUTCDATE(),
    IsWinningBid BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (AuctionId) REFERENCES Auctions(AuctionId),
    FOREIGN KEY (BidderId) REFERENCES Users(UserId),
    INDEX idx_auction_bid (AuctionId),
    INDEX idx_bidder (BidderId)
);
GO

-- Create PropertyImages Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PropertyImages')
CREATE TABLE PropertyImages (
    ImageId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PropertyId UNIQUEIDENTIFIER NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    Caption NVARCHAR(255),
    DisplayOrder INT,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId)
);
GO

-- Create Favorites Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Favorites')
CREATE TABLE Favorites (
    FavoriteId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    PropertyId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
    UNIQUE (UserId, PropertyId)
);
GO

-- Create Notifications Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Notifications')
CREATE TABLE Notifications (
    NotificationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Type NVARCHAR(50), -- AuctionEnding, OutbidNotice, AuctionStarted, etc.
    Title NVARCHAR(255) NOT NULL,
    Message NVARCHAR(MAX),
    PropertyId UNIQUEIDENTIFIER,
    AuctionId UNIQUEIDENTIFIER,
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
    FOREIGN KEY (AuctionId) REFERENCES Auctions(AuctionId)
);
GO

-- Create Indexes
CREATE NONCLUSTERED INDEX idx_properties_city ON Properties(City);
CREATE NONCLUSTERED INDEX idx_properties_type ON Properties(PropertyType);
CREATE NONCLUSTERED INDEX idx_auctions_status ON Auctions(Status);
CREATE NONCLUSTERED INDEX idx_auctions_dates ON Auctions(StartDate, EndDate);
GO

PRINT 'Database schema created successfully!';
