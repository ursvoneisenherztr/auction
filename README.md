# Auction Application

A full-stack auction platform built with **Angular** frontend and **Azure Functions** backend.

## 🚀 Quick Start

### Prerequisites
- Node.js 18+ LTS
- .NET SDK 8.0+
- Azure Functions Core Tools 4.0+
- Azure subscription (for deployment)

### Local Development

```bash
# Install frontend dependencies
cd frontend
npm install
ng serve

# In another terminal, start backend
cd backend
func start
```

Visit `http://localhost:4200` for the frontend (API calls go to `http://localhost:7071/api`)

## 📚 Documentation

- **[Copilot Instructions](.github/copilot-instructions.md)** - How to work with this codebase (for AI and humans)
- **[MCP Servers Setup](.github/MCP_SETUP.md)** - Configure GitHub, Azure, and Playwright integration
- **[MCP Servers Reference](.github/MCP_SERVERS.md)** - Details on each MCP server
- **[MCP Commands Reference](.github/MCP_COMMANDS_REFERENCE.md)** - Commands for Copilot to use
- **[Setup Summary](COPILOT_SETUP_SUMMARY.md)** - Overview of what's configured

## 🏗️ Architecture

### Frontend (Angular)
- Single Page Application at `localhost:4200`
- Feature-based module organization
- Reactive forms and services
- End-to-end tests with Playwright

### Backend (Azure Functions)
- REST APIs running at `localhost:7071/api`
- Serverless functions for auction operations
- Azure Storage and CosmosDB integration
- Async/await patterns throughout

### Communication
```
Frontend (Angular) ←→ HTTP ←→ Azure Functions (Backend)
```

## 🔧 Build Commands

```bash
# Frontend
npm --prefix frontend install        # Install dependencies
npm --prefix frontend start          # Dev server
npm --prefix frontend run build      # Production build
npm --prefix frontend test           # Run tests

# Backend
dotnet restore backend               # Restore packages
dotnet build backend                 # Build
func start                           # Run locally
dotnet test backend                  # Run tests
```

## 🧪 Testing

### Frontend (Playwright)
```bash
cd frontend
npx playwright test              # Run all E2E tests
npx playwright test --headed     # With browser visible
npx playwright test --debug      # Debug mode
```

### Backend (Unit Tests)
```bash
cd backend
dotnet test
```

## 📦 Deployment

### Azure Functions
```bash
cd backend
func azure functionapp publish <function-app-name>
```

### Frontend (Static Hosting)
```bash
cd frontend
npm run build
# Deploy dist/ folder to Azure Static Web Apps or other hosting
```

## 🤖 GitHub Copilot Integration

This project is configured for GitHub Copilot with MCP servers:

- **Playwright**: Write and run E2E tests
- **GitHub**: Manage issues and workflows
- **Azure**: Deploy and monitor resources

See [MCP Setup](.github/MCP_SETUP.md) for configuration.

## 📋 Project Structure

```
auction/
├── frontend/                    # Angular SPA
│   ├── src/
│   ├── angular.json
│   ├── package.json
│   └── playwright.config.ts
├── backend/                     # Azure Functions
│   ├── src/
│   ├── host.json
│   ├── local.settings.json      # Don't commit!
│   └── *.csproj
├── .github/
│   ├── copilot-instructions.md  # 🤖 Copilot guide
│   ├── workflows/
│   │   └── copilot-setup-steps.yml
│   ├── MCP_SERVERS.md
│   ├── MCP_SETUP.md
│   └── MCP_COMMANDS_REFERENCE.md
└── README.md                    # This file
```

## 🔐 Environment Variables

### Frontend
Create `frontend/src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:7071/api'
};
```

### Backend
Create `backend/local.settings.json` (not committed):
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
```

## 🐛 Troubleshooting

**Frontend won't connect to backend?**
- Ensure backend is running on port 7071
- Check CORS in `backend/host.json`
- Verify `apiUrl` in environment file

**Tests failing locally?**
- Clear npm cache: `npm cache clean --force`
- Reinstall: `rm -rf node_modules && npm install`
- Check Node version: `node --version`

**Azure Functions not deploying?**
- Verify Azure CLI login: `az login`
- Check resource group exists
- Ensure Service Principal has permissions

## 📖 Resources

- [Angular Documentation](https://angular.io/docs)
- [Azure Functions Guide](https://learn.microsoft.com/en-us/azure/azure-functions/)
- [Playwright Testing](https://playwright.dev/)
- [GitHub Copilot](https://github.com/features/copilot)

## 📝 License

MIT License - see LICENSE file for details

---

**Ready to start development?** Start with the [Copilot Instructions](.github/copilot-instructions.md)! 🚀
