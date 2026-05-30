# MCP Commands Reference for Copilot

Quick reference for common MCP commands Copilot can execute in your auction project.

## Playwright MCP Commands

### Create Tests
```bash
# Record a new test
npx playwright codegen http://localhost:4200

# Generate tests from recording
npx playwright test --record-new-only
```

### Run Tests
```bash
# Run all tests
npx playwright test

# Run specific test file
npx playwright test auction-details.spec.ts

# Run tests in headed mode (see browser)
npx playwright test --headed

# Run with debug mode
npx playwright test --debug

# Run single test by name
npx playwright test -g "should create auction"
```

### Generate Reports
```bash
# Generate HTML report
npx playwright test --reporter=html

# View report
npx playwright show-report

# Generate JSON report
npx playwright test --reporter=json
```

### Test Maintenance
```bash
# Update snapshots
npx playwright test --update-snapshots

# Check test status
npx playwright test --list
```

---

## GitHub MCP Commands

### Issues
```bash
# Copilot can execute:
"List all open issues"
"Create an issue titled 'Fix auction bid calculations'"
"Show issue #42 details and comments"
"Close issue #42 as resolved"
"Add label 'bug' to issue #15"
"Assign issue #20 to myself"
```

### Workflows
```bash
# Copilot can execute:
"Show me the latest workflow run"
"What are the failed jobs in the last build?"
"Trigger the copilot-setup-steps workflow"
"Show workflow logs for failed jobs"
"List all recent deployment runs"
```

### Repository Info
```bash
# Copilot can execute:
"List branches in the repository"
"Show me recent commits"
"Get the latest release information"
"List repository secrets and variables"
```

---

## Azure MCP Commands

### Azure Functions
```bash
# Copilot can execute:
"Deploy backend to Azure Function App"
"Show me the logs for CreateAuction function"
"List all deployed Azure Functions"
"Check the invocation metrics for PlaceBid"
"View application insights data"
```

### Azure Storage
```bash
# Copilot can execute:
"List all blobs in the auction storage account"
"Get storage account metrics"
"Show blob properties for auction images"
"Create a new container in storage"
```

### Deployment & Monitoring
```bash
# Copilot can execute:
"Deploy the latest function app release"
"Show deployment status and history"
"Check resource group resources"
"Get application performance metrics"
"List function app configuration variables"
```

### Key Vault (if using)
```bash
# Copilot can execute:
"List secrets in Key Vault"
"Show secret version history"
"Update database connection string"
```

---

## Combined Workflow Examples

### Example 1: Bug Fix Workflow
```
1. "Create an issue for investigating slow auction queries"
2. "Show me the Azure Function logs for GetAuctionDetails"
3. "Run Playwright tests to verify the fix"
4. "Deploy the fix to Azure"
5. "Update the GitHub issue with deployment status"
```

### Example 2: New Feature Development
```
1. "Create GitHub issues for auction notifications feature"
2. "Create a Playwright test for the notification flow"
3. "Show recent commits to understand current code patterns"
4. "Deploy changes when tests pass"
5. "Verify deployment metrics in Azure"
```

### Example 3: Performance Investigation
```
1. "Get Azure Function execution metrics for last 24 hours"
2. "Create an issue documenting performance findings"
3. "Run Playwright tests to establish baseline"
4. "Implement optimization"
5. "Run tests again and compare metrics"
```

---

## Setup Requirements for Each MCP Server

| Server | Requires Setup | Environment Variables |
|--------|---|---|
| Playwright | ✓ npm install | None required |
| GitHub | ✗ Built-in | `GITHUB_TOKEN` (auto) |
| Azure | ✓ Azure CLI | `AZURE_SUBSCRIPTION_ID`, `AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET` |

---

## Tips for Using MCP with Copilot

1. **Be Specific**: Instead of "deploy", say "deploy backend to Azure Function App using the latest build"
2. **Combine with Development**: Ask Copilot to make code changes AND run the setup tests together
3. **Check Results**: Always verify deployment logs after asking Copilot to deploy
4. **Use for Monitoring**: Ask Copilot to check logs and metrics before asking for changes
5. **Reference Docs**: Point Copilot to `.github/MCP_SERVERS.md` for complex operations

---

## Disabling/Re-enabling MCP Servers

If you need to disable a server (e.g., during setup):

1. **Temporarily**: Ask Copilot to skip using a specific MCP server
2. **Permanently**: 
   - Remove from `.github/MCP_SERVERS.md`
   - Comment out in `copilot-setup-steps.yml`
   - Remove environment variables from GitHub settings

---

## Error Handling

When Copilot encounters errors:

1. **Playwright Test Failure**: Check selectors, retry with `--debug` flag
2. **GitHub API Error**: Verify token permissions, check rate limits
3. **Azure Deployment Error**: Review `AZURE_CLIENT_SECRET`, check service principal permissions
4. **Timeout**: Retry with longer timeout or simpler command

See `.github/MCP_SETUP.md` for detailed troubleshooting.
