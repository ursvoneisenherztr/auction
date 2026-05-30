# SQL Server Setup Guide for Auction Application

## Database Overview

This application uses **SQL Server** with **Entity Framework Core** as the ORM. The database contains:
- Users (bidders/auction participants)
- Properties (auction items - real estate)
- Auctions (active auction listings)
- Bids (bids placed on auctions)
- PropertyImages, Favorites, Notifications

## Prerequisites

- SQL Server 2019+ (or SQL Server Express)
- SQL Server Management Studio (SSMS) - optional but recommended
- .NET SDK 8.0+

## Setup Steps

### 1. SQL Server Installation

**Option A: SQL Server Express (Free)**
```bash
# Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
# Choose "Express" edition
# Default instance name: SQLEXPRESS
```

**Option B: Local Database (LocalDB)**
```bash
# Included with Visual Studio
# Instance name: (localdb)\mssqllocaldb
```

### 2. Connection String Configuration

Add to `backend/local.settings.json` (local development):
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuctionDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

Or for SQL Server Express:
```
DefaultConnection: "Server=localhost\\SQLEXPRESS;Database=AuctionDB;Trusted_Connection=true;TrustServerCertificate=true;"
```

### 3. Create Database Using SQL Scripts

**Option A: Using SSMS**
1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. Open `backend/database-schema.sql`
4. Execute the script
5. Open `backend/sample-data.sql`
6. Execute the script

**Option B: Using Command Line**
```bash
sqlcmd -S (localdb)\mssqllocaldb -i backend/database-schema.sql
sqlcmd -S (localdb)\mssqllocaldb -i backend/sample-data.sql
```

### 4. Setup Appsettings Configuration

Create `backend/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuctionDB;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

Create `backend/appsettings.Production.json` (for Azure):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:<your-server>.database.windows.net,1433;Initial Catalog=AuctionDB;Persist Security Info=False;User ID=<username>;Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

### 5. Install NuGet Packages

```bash
cd backend

# Entity Framework Core SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0

# For Azure Functions
dotnet add package Microsoft.Azure.Functions.Extensions --version 1.1.0
dotnet add package Microsoft.Azure.WebJobs.Extensions.Storage --version 5.1.0
```

## Entity Framework Core Usage

### Register DbContext in Startup.cs

```csharp
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Auction.API.Data;

[assembly: FunctionsStartup(typeof(Auction.API.Startup))]

namespace Auction.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
            
            builder.Services.AddDbContext<AuctionDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
    }
}
```

### Using DbContext in Azure Functions

```csharp
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Auction.API.Data;
using System.Threading.Tasks;

public class GetPropertiesFunction
{
    private readonly AuctionDbContext _context;

    public GetPropertiesFunction(AuctionDbContext context)
    {
        _context = context;
    }

    [FunctionName("GetProperties")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "properties")] HttpRequest req,
        ILogger log)
    {
        var properties = await _context.Properties
            .Where(p => p.IsActive)
            .ToListAsync();
        
        return new OkObjectResult(properties);
    }
}
```

## Database Maintenance

### Backup Database
```bash
# Using SQL Server Management Studio
# Right-click Database → Tasks → Back Up...

# Or via SSMS
BACKUP DATABASE AuctionDB 
TO DISK = 'C:\Backups\AuctionDB_backup.bak'
WITH INIT, COMPRESSION;
```

### Restore Database
```bash
RESTORE DATABASE AuctionDB 
FROM DISK = 'C:\Backups\AuctionDB_backup.bak';
```

### Query Sample Data
```sql
-- View all properties
SELECT * FROM Properties;

-- View active auctions
SELECT * FROM Auctions WHERE Status = 'Active';

-- View bids for an auction
SELECT a.AuctionNumber, b.Amount, u.FirstName, b.BidTime
FROM Bids b
JOIN Auctions a ON b.AuctionId = a.AuctionId
JOIN Users u ON b.BidderId = u.UserId
ORDER BY b.BidTime DESC;
```

## Azure SQL Database

For production deployment to Azure:

### 1. Create Azure SQL Database
```bash
# Create resource group
az group create --name auction-rg --location eastus

# Create SQL Server
az sql server create \
  --name auction-server \
  --resource-group auction-rg \
  --admin-user sqladmin \
  --admin-password YourPassword123!

# Create database
az sql db create \
  --resource-group auction-rg \
  --server auction-server \
  --name AuctionDB \
  --sku Standard --tier S1

# Allow Azure services
az sql server firewall-rule create \
  --resource-group auction-rg \
  --server auction-server \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

### 2. Deploy Database Schema to Azure
```bash
sqlcmd -S auction-server.database.windows.net \
       -U sqladmin \
       -P YourPassword123! \
       -d AuctionDB \
       -i backend/database-schema.sql
```

### 3. Update Connection String in Azure Key Vault
```bash
az keyvault secret set \
  --vault-name auction-keyvault \
  --name "DefaultConnection" \
  --value "Server=tcp:auction-server.database.windows.net,1433;Initial Catalog=AuctionDB;..."
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Connection refused | Check SQL Server is running, firewall allows access |
| Login failed | Verify username/password in connection string |
| Database not found | Run `database-schema.sql` script |
| Table structure mismatch | Verify Entity Framework models match database schema |
| Azure connection timeout | Check firewall rules, ensure IP is whitelisted |

## Performance Tips

1. **Indexing**: Key indexes are already created in `database-schema.sql`
2. **Pagination**: Use `.Skip()` and `.Take()` for large result sets
3. **Async/Await**: Always use async database operations
4. **Connection Pooling**: SQL Server connection pooling is enabled by default

## Related Files

- `database-schema.sql` - Database creation script
- `sample-data.sql` - Sample data (10 properties)
- `Models/AuctionModels.cs` - Entity models
- `Data/AuctionDbContext.cs` - Entity Framework context
