# Copilot Instructions for Auction Application

## Project Overview

This is a full-stack auction application with:
- **Frontend**: Angular 18+ single-page application
- **Backend**: Azure Functions (C#) serving REST APIs
- **Architecture**: Decoupled frontend and backend that communicate via HTTP

## Directory Structure

```
auction/
├── frontend/              # Angular SPA
│   ├── src/
│   │   ├── app/          # Feature modules and components
│   │   ├── assets/       # Static resources
│   │   └── environments/ # Environment-specific configs
│   ├── angular.json
│   ├── package.json
│   └── tsconfig.json
├── backend/              # Azure Functions
│   ├── src/              # C# function implementations
│   └── host.json         # Functions runtime config
└── .github/copilot-instructions.md
```

## Database Configuration

### SQL Server Setup

**Database**: SQL Server (local or Azure)
**Database Name**: AuctionDB
**ORM**: Entity Framework Core 8.0

**Quick Setup**:
1. Create database: Run `backend/database-schema.sql` in SQL Server Management Studio
2. Load sample data: Run `backend/sample-data.sql` (10 Swiss properties)
3. Configure connection: Add to `backend/local.settings.json`:
   ```json
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuctionDB;Trusted_Connection=true;"
   ```

**Database Contents**:
- **Properties** (10 records): Swiss real estate properties with pricing and details
- **Auctions** (10 records): Active forced sales/auctions
- **Users** (3 records): Sample bidders/auction participants
- **Bids**: Bid history and tracking
- **PropertyImages, Favorites, Notifications**: Supporting tables

**Sample Data**: 
Properties from Zurich, Bern, Basel, Lucerne, Geneva, Lausanne, St. Gallen, Winterthur, Fribourg, Neuchâtel
(CHF 320,000 - CHF 1,250,000 price range)

**Documentation**:
- `.backend/DATABASE_QUICK_START.md` - 5-minute setup
- `.backend/SQL_SERVER_SETUP.md` - Complete setup guide
- `.backend/PROPERTY_DATA_IMPORT.md` - Real data import options

## Build & Development Commands

### Frontend (Angular)

```bash
cd frontend

# Install dependencies
npm install

# Start development server (http://localhost:4200)
ng serve
# or: npm start

# Build for production
ng build --configuration production

# Run unit tests (Jasmine/Karma)
ng test

# Run a specific test file
ng test --include='**/feature.component.spec.ts'

# Run e2e tests (Cypress/Protractor)
ng e2e

# Run linting (ESLint)
ng lint
# or: npm run lint

# Format code (Prettier)
npm run format
```

### Backend (Azure Functions)

```bash
cd backend

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Run locally with Azure Functions emulator
func start

# Run unit tests
dotnet test

# Publish to Azure
func azure functionapp publish <function-app-name>
```

### Database Management

```bash
# Create database (SQL Server Management Studio or command line)
# Option 1: SSMS - Open database-schema.sql and execute
# Option 2: Command line
sqlcmd -S (localdb)\mssqllocaldb -i backend/database-schema.sql

# Load sample data
sqlcmd -S (localdb)\mssqllocaldb -d AuctionDB -i backend/sample-data.sql

# Query sample data
sqlcmd -S (localdb)\mssqllocaldb -d AuctionDB
> SELECT COUNT(*) FROM Properties;
> SELECT COUNT(*) FROM Auctions;
```

### Combined Commands (from root)

```bash
# Install dependencies for both frontend and backend
npm --prefix frontend install
dotnet restore backend

# Run both dev servers (requires two terminals)
npm --prefix frontend start &
(cd backend && func start)

# Build both for production
npm --prefix frontend run build
dotnet build backend
```

## High-Level Architecture

### Frontend Architecture

- **Module Organization**: Feature-based modules (e.g., `auction`, `bidding`, `user`)
- **Shared Module**: Common components, pipes, directives, guards in `shared/` folder
- **HTTP Communication**: HttpClientModule with interceptors for:
  - Authentication headers
  - Error handling
  - Request/response transformation
- **State Management**: Consider NgRx or Signals (Angular 17+) for complex state
- **Routing**: Lazy-loaded feature modules with route guards for protected pages
- **Services**: Data services for API calls in `services/` folder

### Backend Architecture

- **Function Organization**: Each Azure Function handles one business operation (e.g., `CreateAuction`, `PlaceBid`, `GetAuctionDetails`)
- **Dependency Injection**: Use Azure Functions built-in DI container via `Startup.cs`
- **Data Access**: Repository pattern for database operations
- **Models**: DTOs for request/response contracts in separate `Models/` folder
- **Error Handling**: Custom exception classes with meaningful HTTP status codes
- **CORS**: Configure in `host.json` to allow frontend domain

### API Contract

Frontend communicates with backend via REST endpoints:
```
GET    /api/auctions                    - List all auctions
GET    /api/auctions/{id}               - Get auction details
POST   /api/auctions                    - Create new auction
POST   /api/auctions/{id}/bids          - Place a bid
GET    /api/auctions/{id}/bids          - Get auction bids
DELETE /api/auctions/{id}               - Close/delete auction
```

Response format:
```json
{
  "success": true,
  "data": { /* payload */ },
  "error": null,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

Error response:
```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "AUCTION_NOT_FOUND",
    "message": "Auction with ID 123 does not exist"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Key Conventions

### Frontend (Angular)

- **Naming**: PascalCase for classes/components (e.g., `AuctionDetailComponent`), camelCase for properties/methods
- **Component Structure**:
  - Split large components into smaller, reusable ones
  - Use `OnInit` for initialization, `OnDestroy` for cleanup (unsubscribe)
  - Use reactive forms (`FormGroup`, `FormControl`) instead of template-driven forms
- **Services**: Use `providedIn: 'root'` for singletons, avoid circular dependencies
- **HTTP**: Always use typed responses, handle errors in services not components
- **Styling**: Keep component styles in `component.scss`, global styles in `styles.scss`
- **File Naming**: `feature.component.ts`, `feature.service.ts`, `feature.model.ts`
- **Observables**: Unsubscribe in `ngOnDestroy` or use `async` pipe in templates

### Backend (Azure Functions)

- **Naming**: PascalCase for classes and methods (standard C#)
- **Function Names**: Match HTTP method + resource (e.g., `CreateAuction`, `GetAuctionById`)
- **Logging**: Use `ILogger` injected into functions, avoid `Console.WriteLine`
- **Return Types**: Use `IActionResult` or typed task results for consistency
- **Validation**: Validate inputs at function entry point, return 400 for bad requests
- **Async/Await**: All I/O operations must be async
- **Connection Strings**: Store in `local.settings.json` (local) and Azure Key Vault (production)
- **Dependency Injection**: Register services in `Startup.cs`, inject via constructor

### Shared Conventions

- **Environment Config**: Frontend uses `environments/environment.ts` to set API base URL
- **Error Codes**: Define standardized error codes (e.g., `AUCTION_NOT_FOUND`, `INVALID_BID`) used by both frontend and backend
- **Timestamps**: All timestamps in UTC ISO 8601 format
- **IDs**: Use GUIDs for entities (or UUIDs in frontend if needed)

## Development Workflow

1. **Feature Branch**: Create branch from `main` (e.g., `feature/auction-details`)
2. **Frontend & Backend**: Develop API contract first (decide request/response DTOs), then implement both sides in parallel
3. **Testing**: Write unit tests as you code, test both frontend and backend independently
4. **Integration**: Test frontend + backend together locally before pushing
5. **Code Review**: Ensure linting passes, tests pass, no console errors
6. **Deployment**: Merge to `main`, which triggers CI/CD to deploy frontend to hosting and backend to Azure

## Testing Strategy

### Frontend
- **Unit Tests**: Test components, services, guards in isolation using Jasmine
- **Integration Tests**: Use TestBed to test component + service interaction
- **E2E Tests**: Cypress tests for user workflows (login, create auction, place bid)
- **Coverage**: Aim for 80%+ line coverage on critical paths

### Backend
- **Unit Tests**: xUnit tests for services, repositories, business logic
- **Integration Tests**: Test functions with mocked Azure Storage/Cosmos DB
- **Mocking**: Use Moq for dependency mocking

## Debugging

### Frontend
- Use Angular DevTools Chrome extension
- Breakpoints in Chrome DevTools (F12 → Sources)
- Check Network tab for API calls and responses
- Use `ng serve --configuration development` for better error messages

### Backend
- Use VSCode debugger or Visual Studio debugger
- Breakpoints in Azure Functions emulator
- Check Application Insights logs in Azure Portal for production
- Use `func start --debug` for debugging locally

## Environment Setup

### Prerequisites
- Node.js 18+ LTS
- npm 9+
- .NET SDK 7.0+ (for C#)
- Azure Functions Core Tools (`npm install -g azure-functions-core-tools@4 --unsafe-perm true`)
- VSCode or Visual Studio 2022

### Local Development

**Frontend**:
```bash
cd frontend
npm install
ng serve
```

**Backend**:
```bash
cd backend
dotnet restore
dotnet build
func start
```

Visit `http://localhost:4200` (frontend) and `http://localhost:7071` (backend functions).

### Environment Files

**Frontend** (`frontend/src/environments/`):
- `environment.ts`: Local development (apiUrl: `http://localhost:7071/api`)
- `environment.prod.ts`: Production (apiUrl: Azure Function App URL)

**Backend** (`backend/local.settings.json`):
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
```

## Important Notes

- Frontend is served independently; backend functions are not responsible for serving static files
- CORS must be configured in `host.json` to allow frontend domain to call backend
- Use Azure Storage Emulator or local mock for development (Azurite)
- Environment-specific configs must never be committed (use `.gitignore` for `local.settings.json`)

## MCP Servers Integration

This project uses three MCP servers to enhance Copilot's capabilities:

### 1. Playwright
- Automated browser testing for Angular frontend
- Use for E2E test creation and execution
- Reference: `.github/MCP_SERVERS.md` → Playwright section

### 2. GitHub
- Manage issues and CI/CD workflows
- Track bugs, features, and progress
- Reference: `.github/MCP_SERVERS.md` → GitHub MCP Server section

### 3. Azure
- Deploy Azure Functions and manage resources
- Monitor logs and metrics
- Requires credentials in `copilot` environment
- Reference: `.github/MCP_SERVERS.md` → Azure MCP Server section

See [`.github/MCP_SERVERS.md`](.github/MCP_SERVERS.md) for detailed configuration and usage examples.

## Resources

- [Angular Documentation](https://angular.io/docs)
- [Azure Functions C# Developer Guide](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library)
- [REST API Best Practices](https://restfulapi.net/)
- [Playwright Documentation](https://playwright.dev/)
- [GitHub Issues REST API](https://docs.github.com/en/rest/issues)
- [Azure CLI Reference](https://learn.microsoft.com/en-us/cli/azure/reference-index)
