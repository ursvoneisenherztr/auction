# Property Data Import Guide

This guide explains how to populate the auction database with real property data.

## Sample Data (Already Provided)

The project includes **10 sample Swiss properties** in `backend/sample-data.sql` covering:
- Zurich apartments
- Bern houses
- Basel commercial spaces
- Lucerne villas
- Geneva penthouses
- And more...

### Load Sample Data

```bash
# Using SQL Server Management Studio
# Open sample-data.sql and execute

# Or via command line
sqlcmd -S (localdb)\mssqllocaldb -d AuctionDB -i backend/sample-data.sql
```

## Scraping Real Property Data

To import properties from `zwangsversteigerung.ch` or similar sources:

### Method 1: Web Scraping with HtmlAgilityPack

Create `backend/Services/PropertyScraperService.cs`:

```csharp
using HtmlAgilityPack;
using Auction.API.Models;
using Auction.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class PropertyScraperService
{
    private readonly HttpClient _httpClient;
    private readonly AuctionDbContext _context;

    public PropertyScraperService(HttpClient httpClient, AuctionDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public async Task<List<Property>> ScrapPropertiesAsync(string url, int limit = 10)
    {
        var properties = new List<Property>();
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            // Select property elements (adjust selectors based on actual website structure)
            var propertyNodes = doc.DocumentNode
                .SelectNodes("//div[@class='property-item']")
                .Take(limit);

            foreach (var node in propertyNodes)
            {
                var property = new Property
                {
                    PropertyId = Guid.NewGuid(),
                    Title = ExtractText(node, ".property-title"),
                    Address = ExtractText(node, ".property-address"),
                    City = ExtractText(node, ".property-city"),
                    Description = ExtractText(node, ".property-description"),
                    PropertyType = ExtractText(node, ".property-type") ?? "House",
                    StartPrice = decimal.TryParse(
                        ExtractText(node, ".property-price")?.Replace(",", "."), 
                        out var price) ? price : 0,
                    ImageUrl = ExtractAttribute(node, "img", "src"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                properties.Add(property);
            }

            // Save to database
            await _context.Properties.AddRangeAsync(properties);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Scraping error: {ex.Message}");
        }

        return properties;
    }

    private string ExtractText(HtmlNode node, string selector)
    {
        return node.SelectSingleNode(selector)?.InnerText?.Trim();
    }

    private string ExtractAttribute(HtmlNode node, string tagName, string attrName)
    {
        return node.SelectSingleNode(tagName)?.GetAttributeValue(attrName, null);
    }
}
```

### Method 2: API-Based Import (Preferred)

If the website has an API:

```csharp
public async Task ImportFromApiAsync(string apiUrl)
{
    var response = await _httpClient.GetAsync(apiUrl);
    var json = await response.Content.ReadAsStringAsync();
    
    var propertyData = JsonConvert.DeserializeObject<List<PropertyDto>>(json);
    
    var properties = propertyData.Select(p => new Property
    {
        Title = p.Title,
        Address = p.Address,
        City = p.City,
        StartPrice = p.Price,
        PropertyType = p.Type,
        // ... map other fields
    }).ToList();

    await _context.Properties.AddRangeAsync(properties);
    await _context.SaveChangesAsync();
}
```

### Method 3: Manual CSV Import

Create import from CSV file:

```csharp
using CsvHelper;
using System.Globalization;

public async Task ImportFromCsvAsync(string filePath)
{
    var properties = new List<Property>();

    using (var reader = new StreamReader(filePath))
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        var records = csv.GetRecords<PropertyCsvDto>();

        foreach (var record in records)
        {
            properties.Add(new Property
            {
                PropertyId = Guid.NewGuid(),
                Title = record.Title,
                Address = record.Address,
                City = record.City,
                StartPrice = decimal.Parse(record.Price),
                PropertyType = record.PropertyType,
                // ... map other fields
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });
        }
    }

    await _context.Properties.AddRangeAsync(properties);
    await _context.SaveChangesAsync();
}
```

## NuGet Packages Required

```bash
# For web scraping
dotnet add package HtmlAgilityPack

# For JSON parsing
dotnet add package Newtonsoft.Json

# For CSV import
dotnet add package CsvHelper
```

## Azure Function for Property Import

Create `backend/Functions/ImportPropertiesFunction.cs`:

```csharp
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Auction.API.Data;
using Auction.API.Services;
using System.Threading.Tasks;

public class ImportPropertiesFunction
{
    private readonly AuctionDbContext _context;
    private readonly PropertyScraperService _scraper;

    public ImportPropertiesFunction(AuctionDbContext context, PropertyScraperService scraper)
    {
        _context = context;
        _scraper = scraper;
    }

    [FunctionName("ImportProperties")]
    public async Task Run(
        [TimerTrigger("0 0 * * * *")] TimerInfo myTimer, // Daily
        ILogger log)
    {
        log.LogInformation("Starting property import...");

        try
        {
            var properties = await _scraper.ScrapPropertiesAsync(
                "https://zwangsversteigerung.ch/",
                limit: 10);

            log.LogInformation($"Imported {properties.Count} properties");
        }
        catch (Exception ex)
        {
            log.LogError($"Import failed: {ex.Message}");
        }
    }
}
```

## Legal Considerations

⚠️ **Important**: When scraping websites, ensure you:

1. **Check Terms of Service**: Verify the website allows scraping
2. **Respect robots.txt**: Follow crawling rules
3. **Rate Limiting**: Don't overwhelm the server with requests
4. **Data Privacy**: Comply with GDPR and local data protection laws
5. **Attribution**: Give credit if required
6. **Licensing**: Verify you have rights to the data

### Example: robots.txt Check

```bash
# Check if scraping is allowed
curl https://zwangsversteigerung.ch/robots.txt
```

## Sample Data Structure

The sample data includes these property types:

| Type | Count | Example |
|------|-------|---------|
| Apartment | 3 | Zurich, Geneva, Lausanne |
| House | 5 | Bern, Lucerne, St. Gallen, etc. |
| Commercial | 2 | Basel, Fribourg |
| Land | 1 | Winterthur |

**Price Range**: CHF 320,000 - CHF 1,250,000

## Scheduled Data Updates

To keep property listings fresh, use Azure Functions scheduled triggers:

```csharp
[TimerTrigger("0 0 12 * * *")] // Every day at noon UTC
public async Task RunAsync(TimerInfo myTimer, ILogger log)
{
    // Daily property update
}
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Duplicate properties | Add `UNIQUE` constraint on address |
| Missing data fields | Use null coalescing `field ?? "N/A"` |
| Slow scraping | Implement async/await, use parallel processing |
| Blocked by website | Rotate user agents, add delays, use proxy |

## Next Steps

1. Verify sample data is loaded: `SELECT COUNT(*) FROM Properties;`
2. Test API endpoints with sample data
3. Implement scraper for real data when ready
4. Set up scheduled import job in Azure Functions

## Resources

- HtmlAgilityPack: https://html-agility-pack.net/
- CsvHelper: https://joshclose.github.io/CsvHelper/
- Web Scraping Best Practices: https://www.scrapehero.com/web-scraping-best-practices/
