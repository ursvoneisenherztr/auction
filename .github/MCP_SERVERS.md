# MCP Servers Configuration for Auction Application

This project is configured to use the following MCP (Model Context Protocol) servers for enhanced Copilot capabilities:

## 1. Playwright MCP Server

**Purpose**: Automated browser testing for the Angular frontend

**Setup**: Installed via `copilot-setup-steps.yml`

**Usage**: Create and run Playwright tests for end-to-end testing
```bash
cd frontend
npx playwright test
npx playwright test --headed  # Run with browser visible
npx playwright test --debug   # Debug mode
```

**Test Location**: `frontend/tests/` or `frontend/e2e/`

**Configuration File**: `frontend/playwright.config.ts`

**Common Commands**:
- Record test: `npx playwright codegen http://localhost:4200`
- Run specific test: `npx playwright test path/to/test.spec.ts`
- Generate report: `npx playwright test --reporter=html`

---

## 2. GitHub MCP Server

**Purpose**: Managing issues, pull requests, CI/CD workflows, and repository configuration

**Capabilities**:
- List, search, and filter GitHub issues
- View issue details, comments, and labels
- Create and update issues
- Manage pull requests and reviews
- Trigger workflows and check build status
- Access repository settings and secrets

**Setup**: Automatically available when Copilot connects to your GitHub repository

**Configuration**: 
- Issues are managed through `github.com/<owner>/<repo>/issues`
- Workflows located in `.github/workflows/`
- Environment secrets configured in repository settings → Environments → `copilot`

**Useful for Copilot**:
- "Create an issue for testing the auction creation flow"
- "What are the currently open issues?"
- "Check the status of the latest CI/CD run"
- "List failed workflow runs and their error logs"

---

## 3. Azure MCP Server

**Purpose**: Managing Azure Functions, Storage, Key Vault, and deployments

**Setup**: Requires Azure CLI authentication in the environment
```bash
az login
az account set --subscription "<subscription-id>"
```

**Capabilities**:
- Deploy Azure Functions
- Manage Azure Storage (Blob, Queue, Table)
- View Azure Function logs and metrics
- Manage Azure Key Vault secrets
- Monitor application performance

**Configuration Files**:
- Function runtime: `backend/host.json`
- Local settings: `backend/local.settings.json` (not committed)
- Production settings: Azure Portal → Function App → Configuration

**Environment Variables for Copilot**:
Set these in repository settings → Environments → `copilot`:
- `AZURE_SUBSCRIPTION_ID` - Your Azure subscription ID
- `AZURE_RESOURCE_GROUP` - Resource group name
- `AZURE_FUNCTION_APP_NAME` - Name of the Function App

**Useful for Copilot**:
- "Deploy the backend to Azure"
- "Check Azure Function logs for errors"
- "List all blobs in the storage account"
- "Update secrets in Key Vault"
- "Get metrics for the last 24 hours"

---

## Authentication & Permissions

For MCP servers to work with Copilot in your cloud agent environment:

### GitHub
- Copilot uses the repository's default GitHub token
- No additional setup needed

### Azure
1. Create an Azure Service Principal:
   ```bash
   az ad sp create-for-rbac --name "copilot-agent" \
     --role Contributor \
     --scopes /subscriptions/<subscription-id>/resourceGroups/<resource-group>
   ```

2. Add the credentials as GitHub Actions secrets:
   - `AZURE_SUBSCRIPTION_ID`
   - `AZURE_TENANT_ID`
   - `AZURE_CLIENT_ID`
   - `AZURE_CLIENT_SECRET`

3. Reference them in `.github/workflows/` or in the `copilot` environment

### Playwright
- Runs locally in the Copilot environment
- No authentication needed
- Requires `frontend/playwright.config.ts` configuration

---

## Workflow Integration

When Copilot operates:

1. **Setup Phase**: `copilot-setup-steps.yml` runs to install all dependencies
2. **Development Phase**: Copilot can use MCP servers to:
   - Check Azure Function logs for debugging
   - Create GitHub issues for tracking work
   - Run Playwright tests to validate changes
3. **Deployment Phase**: Copilot can trigger Azure deployments using MCP commands

---

## Example Scenarios

### Scenario 1: Fixing a bug
```
1. Copilot checks GitHub issues using MCP GitHub server
2. Creates a feature branch
3. Makes code changes
4. Runs Playwright tests to verify UI changes
5. Checks Azure Function logs to debug backend
6. Creates a pull request with findings
```

### Scenario 2: Adding new feature
```
1. Copilot checks Azure metrics for performance baseline
2. Implements Angular component
3. Adds Azure Function for new operation
4. Runs Playwright test for new flow
5. Deploys to staging via Azure MCP server
6. Verifies deployment success
```

---

## Troubleshooting

**Playwright tests fail**: Check browser compatibility, ensure proper test selectors, review `playwright.config.ts`

**Azure deployment fails**: Verify credentials in `copilot` environment, check permissions on service principal, ensure resource group exists

**GitHub issues not accessible**: Ensure `GITHUB_TOKEN` has `read:issues` and `write:issues` permissions

---

## Resources

- [Playwright Documentation](https://playwright.dev/)
- [GitHub Issues REST API](https://docs.github.com/en/rest/issues)
- [Azure CLI Reference](https://learn.microsoft.com/en-us/cli/azure/reference-index)
- [MCP Protocol Documentation](https://modelcontextprotocol.io/)
