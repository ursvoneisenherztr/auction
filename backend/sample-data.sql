-- Sample Data for Auction Database
-- 10 Example Properties from Swiss Property Auctions

USE AuctionDB;
GO

-- Insert Sample Users
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, PhoneNumber, City, PostalCode, Country)
VALUES
    ('johndoe', 'john.doe@example.com', 'hash_password_123', 'John', 'Doe', '+41791234567', 'Zurich', '8000', 'Switzerland'),
    ('mariasmith', 'maria.smith@example.com', 'hash_password_456', 'Maria', 'Smith', '+41791234568', 'Bern', '3011', 'Switzerland'),
    ('invest2024', 'investor@example.com', 'hash_password_789', 'Real', 'Estate Investor', '+41791234569', 'Basel', '4001', 'Switzerland');
GO

-- Insert Sample Properties (10 properties inspired by Swiss forced sales)
INSERT INTO Properties (
    Title, Description, Address, City, PostalCode, Region, Country, PropertyType,
    SquareMeters, Rooms, Bedrooms, Bathrooms, YearBuilt, Condition,
    StartPrice, EstimatedValue, Latitude, Longitude, ImageUrl
)
VALUES
-- Property 1: Zurich Apartment
('Moderne Wohnung in Zurich-Wiedikon', 
 'Geschmackvolle 3.5-Zimmer Wohnung mit Balkon, renovierte Küche, Parkettboden', 
 'Wiedikon Strasse 45', 'Zurich', '8003', 'Zurich', 'Switzerland', 'Apartment',
 95.5, 3.5, 2, 1, 1998, 'Good',
 450000, 550000, 47.3588, 8.5192, 'https://example.com/property1.jpg'),

-- Property 2: Bern House
('Einfamilienhaus in Bern-Bethlehem',
 'Gepflegtes Einfamilienhaus mit Garten, 6 Zimmer, Dachausbau möglich',
 'Bethlehem Weg 12', 'Bern', '3014', 'Bern', 'Switzerland', 'House',
 180.0, 6, 4, 2, 1975, 'Fair',
 650000, 750000, 46.9382, 7.4474, 'https://example.com/property2.jpg'),

-- Property 3: Basel Commercial Space
('Geschäftsraum in Basel-Stadt',
 'Attraktiver Laden mit Schaufenster, ideal für Einzelhandel oder Büro',
 'Freie Strasse 98', 'Basel', '4001', 'Basel', 'Switzerland', 'Commercial',
 72.0, 2, 0, 1, 1985, 'Good',
 380000, 450000, 47.5596, 7.5886, 'https://example.com/property3.jpg'),

-- Property 4: Lucerne Villa
('Villa mit See-Aussicht in Lucerne',
 'Luxus-Villa mit Seeblick, 8 Zimmer, private Liegenschaft mit Seezugang',
 'Seefront Promenade 34', 'Lucerne', '6005', 'Lucerne', 'Switzerland', 'House',
 320.0, 8, 5, 3, 2005, 'Excellent',
 1250000, 1500000, 47.0501, 8.3093, 'https://example.com/property4.jpg'),

-- Property 5: Geneva Penthouse
('Exklusives Penthouse in Genève',
 '4-Zimmer Penthouse mit Terrasse, modernes Design, City-Blick',
 'Rue de la Paix 156', 'Geneva', '1201', 'Geneva', 'Switzerland', 'Apartment',
 160.0, 4, 2, 2, 2015, 'Excellent',
 890000, 1100000, 46.2017, 6.1432, 'https://example.com/property5.jpg'),

-- Property 6: Lausanne Apartment
('Gemütliche Altbauwohnung in Lausanne',
 '2.5-Zimmer Wohnung, Eichenparkett, originalgetreue Stuckdecken',
 'Avenue de la Gare 23', 'Lausanne', '1001', 'Vaud', 'Switzerland', 'Apartment',
 68.5, 2.5, 1, 1, 1920, 'Fair',
 350000, 420000, 46.5197, 6.6323, 'https://example.com/property6.jpg'),

-- Property 7: St. Gallen Townhouse
('Renoviertes Townhouse in St. Gallen',
 'Stadthaus mit moderner Ausstattung, 5 Zimmer, sonnig und zentral',
 'Markt Platz 7', 'St. Gallen', '9000', 'St. Gallen', 'Switzerland', 'House',
 145.0, 5, 3, 2, 1980, 'Good',
 580000, 650000, 47.4241, 9.3771, 'https://example.com/property7.jpg'),

-- Property 8: Winterthur Land Property
('Grundstück mit Entwicklungspotenzial',
 'Bauland in Winterthur, Zone für Wohnbau, ideal für Projekt-Entwickler',
 'Industrie Strasse 44', 'Winterthur', '8400', 'Zurich', 'Switzerland', 'Land',
 850.0, 0, 0, 0, NULL, 'Good',
 320000, 400000, 47.5034, 8.7275, 'https://example.com/property8.jpg'),

-- Property 9: Fribourg Mixed Property
('Wohn- und Geschäftshaus in Fribourg',
 'Gemischtes Objekt mit Wohnung oben und Ladenlokal unten',
 'Rue de Romont 78', 'Fribourg', '1700', 'Fribourg', 'Switzerland', 'Commercial',
 210.0, 6, 3, 2, 1990, 'Good',
 720000, 850000, 46.8045, 7.1585, 'https://example.com/property9.jpg'),

-- Property 10: Neuchâtel Chalet
('Gemütliches Chalet in Neuchâtel',
 'Bergchalet mit Kachelofen, 4 Zimmer, Naturstein-Fassade, ruhige Lage',
 'Alpen Weg 21', 'Neuchâtel', '2000', 'Neuchâtel', 'Switzerland', 'House',
 120.0, 4, 2, 1, 1960, 'Fair',
 420000, 500000, 46.9921, 6.9281, 'https://example.com/property10.jpg');
GO

-- Insert Sample Auctions (for the 10 properties)
INSERT INTO Auctions (
    PropertyId, AuctionNumber, StartDate, EndDate, Status, MinimumBid, 
    SellerName, CourtName, AuctionType, Reason
)
SELECT TOP 10 
    PropertyId,
    'ZVG-2024-' + FORMAT(ROW_NUMBER() OVER (ORDER BY CreatedAt), '0000'),
    DATEADD(DAY, -10, GETUTCDATE()),
    DATEADD(DAY, 20, GETUTCDATE()),
    'Active',
    StartPrice * 0.9,
    'Betreibungsamt Zurich',
    'Bezirksgericht Zurich',
    'Forced Sale',
    'Betreibungsverfahren'
FROM Properties
ORDER BY CreatedAt;
GO

-- Verify Data Insert
SELECT 'Users Created' AS [Status], COUNT(*) AS [Count] FROM Users
UNION ALL
SELECT 'Properties Created', COUNT(*) FROM Properties
UNION ALL
SELECT 'Auctions Created', COUNT(*) FROM Auctions;
GO

PRINT 'Sample data inserted successfully!';
PRINT 'Total Properties: 10';
PRINT 'Total Auctions: 10';
