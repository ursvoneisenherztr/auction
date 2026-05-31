using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Auction.API.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        // Add CORS support
        services.AddCors(options => {
            options.AddDefaultPolicy(builder => {
                builder
                    .WithOrigins("http://localhost:4200", "http://localhost:23584")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        
        // Add DbContext with SQL Server
        var connectionString = Environment.GetEnvironmentVariable("DatabaseConnection")
            ?? "Server=(localdb)\\mssqllocaldb;Database=AuctionDb;Integrated Security=true;TrustServerCertificate=true;";
        
        services.AddDbContext<AuctionDbContext>(options =>
            options.UseSqlServer(connectionString)
        );
    })
    .Build();

host.Run();
