# SQL Server Database Setup - Quick Start

## Overview

Your auction application uses **SQL Server** as the database backend. This guide gets you started quickly.

## 5-Minute Quick Start

### Step 1: Create Database
```bash
# Using SQL Server Management Studio (SSMS):
# 1. Open SSMS
# 2. File → Open → File
# 3. Select: backend/database-schema.sql
# 4. Click Execute (or press F5)
# 5. Repeat with backend/sample-data.sql
```

### Step 2: Configure Connection String
Add to `backend/local.settings.json`:
```json
{
  "Values": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuctionDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Step 3: Install Dependencies
```bash
cd backend
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
```

### Step 4: Verify Data
```bash
# Query in SSMS
SELECT COUNT(*) AS PropertyCount FROM Properties;
SELECT COUNT(*) AS AuctionCount FROM Auctions;
```

## What's Included

✅ **Database Schema** - 7 tables, proper indexes
✅ **Sample Data** - 10 Swiss properties
✅ **Entity Models** - C# classes for all tables
✅ **DbContext** - Entity Framework configuration
✅ **Documentation** - SQL Server setup guide + Property import guide

## Database Schema

```
Properties (10 records)
├── PropertyId (GUID, Primary Key)
├── Title, Description, Address, City
├── Type, SquareMeters, Rooms, Bedrooms
├── StartPrice, EstimatedValue
└── Latitude, Longitude, ImageUrl

Auctions (10 records)
├── AuctionId (GUID, Primary Key)
├── PropertyId (Foreign Key)
├── AuctionNumber, Status, Dates
├── MinimumBid, CurrentHighestBid
└── Seller, Court, Reason

Users (3 records)
├── Username, Email, PasswordHash
├── Name, Contact, Address
└── IsActive flag

Bids
├── AuctionId, BidderId, Amount
├── BidTime, IsWinningBid
└── Cascade delete with auction

Plus: PropertyImages, Favorites, Notifications
```

## Sample Data Preview

**10 Properties across Switzerland:**
1. Zurich - Modern Apartment - CHF 450,000
2. Bern - Family House - CHF 650,000
3. Basel - Commercial Space - CHF 380,000
4. Lucerne - Lake View Villa - CHF 1,250,000
5. Geneva - Penthouse - CHF 890,000
6. Lausanne - Altbau Apartment - CHF 350,000
7. St. Gallen - Townhouse - CHF 580,000
8. Winterthur - Land (Development) - CHF 320,000
9. Fribourg - Mixed Property - CHF 720,000
10. Neuchâtel - Mountain Chalet - CHF 420,000

## Files Reference

| File | Purpose |
|------|---------|
| `database-schema.sql` | Create all tables with indexes |
| `sample-data.sql` | Insert 10 sample properties |
| `Models/AuctionModels.cs` | Entity Framework models |
| `Data/AuctionDbContext.cs` | DbContext configuration |
| `SQL_SERVER_SETUP.md` | Detailed setup guide |
| `PROPERTY_DATA_IMPORT.md` | How to import real data |

## For Azure Deployment

See `SQL_SERVER_SETUP.md` → "Azure SQL Database" section for:
- Creating Azure SQL Server
- Deploying schema to cloud
- Configuring connection strings
- Key Vault integration

## Key Features

🔒 **Security**: Parameterized queries prevent SQL injection
📊 **Performance**: Strategic indexes on City, Type, Status, Dates
🔄 **Relationships**: Foreign keys with proper cascade rules
📱 **Scalability**: Ready for thousands of properties and auctions
🌍 **Localization**: Support for Swiss locations, multi-language

## Next Steps

1. ✅ Run `database-schema.sql` to create tables
2. ✅ Run `sample-data.sql` to load sample data
3. ✅ Add connection string to `local.settings.json`
4. ✅ Install NuGet packages (see SQL_SERVER_SETUP.md)
5. ⏭️ Create API endpoints to query data
6. ⏭️ Build frontend to display properties
7. ⏭️ Implement bidding functionality

## Troubleshooting

**"Login failed for user"**
→ Check connection string, verify SQL Server is running

**"Database does not exist"**
→ Run `database-schema.sql` first

**"Cannot insert duplicate key"**
→ Sample data already loaded, run with fresh database

**"Entity type has no key defined"**
→ Verify `AuctionDbContext.cs` is in `Models/` and `Data/` folders

## Support Files

- Full setup guide: `backend/SQL_SERVER_SETUP.md`
- Data import options: `backend/PROPERTY_DATA_IMPORT.md`
- Architecture docs: `.github/copilot-instructions.md`

---

**Ready to query property auctions!** 🏠

Database is pre-configured with 10 Swiss properties. Start building API endpoints to retrieve and manage auction data.
