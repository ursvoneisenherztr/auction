# MCP Setup Guide for Copilot

This guide helps you configure MCP servers for Copilot to access GitHub, Azure, and Playwright resources.

## Prerequisites

- GitHub repository created and pushed
- Azure subscription with resource group
- Azure CLI installed (`az login` configured)

## Step 1: Create GitHub Secrets for Azure

If using Azure MCP server, create a Service Principal for Copilot:

```bash
# Set your subscription ID and resource group
SUBSCRIPTION_ID="<your-subscription-id>"
RESOURCE_GROUP="<your-resource-group>"
FUNCTION_APP_NAME="auction-api"

# Create Service Principal
az ad sp create-for-rbac \
  --name "copilot-auction-agent" \
  --role Contributor \
  --scopes /subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP
```

This will output:
```json
{
  "appId": "...",
  "displayName": "copilot-auction-agent",
  "password": "...",
  "tenant": "..."
}
```

## Step 2: Add GitHub Repository Secrets

1. Go to your GitHub repository → **Settings** → **Secrets and variables** → **Actions**

2. Add these secrets:
   - `AZURE_SUBSCRIPTION_ID`: Your subscription ID
   - `AZURE_TENANT_ID`: From Service Principal output
   - `AZURE_CLIENT_ID`: From `appId`
   - `AZURE_CLIENT_SECRET`: From `password`

## Step 3: Create Copilot Environment Variables

1. Go to **Settings** → **Environments** (or create new one called "copilot" if it doesn't exist)

2. Add **Environment variables**:
   ```
   AZURE_SUBSCRIPTION_ID = <your-subscription-id>
   AZURE_RESOURCE_GROUP = <your-resource-group>
   AZURE_FUNCTION_APP_NAME = auction-api
   AZURE_STORAGE_ACCOUNT = <storage-account-name>
   ```

3. Reference secrets with `${{ secrets.SECRET_NAME }}` in workflows

## Step 4: Configure Azure Resources

Create the Azure resources Copilot will manage:

```bash
# Create Function App
az functionapp create \
  --resource-group $RESOURCE_GROUP \
  --consumption-plan-location eastus \
  --runtime dotnet \
  --runtime-version 8.0 \
  --functions-version 4 \
  --name $FUNCTION_APP_NAME \
  --storage-account <storage-account-name>

# Create Storage Account
az storage account create \
  --resource-group $RESOURCE_GROUP \
  --name auctionstorage \
  --location eastus \
  --sku Standard_LRS

# Create Application Insights
az monitor app-insights component create \
  --app auction-insights \
  --location eastus \
  --resource-group $RESOURCE_GROUP \
  --application-type web
```

## Step 5: Deploy Initial Backend

```bash
# From backend directory
cd backend
func azure functionapp publish $FUNCTION_APP_NAME
```

## Step 6: Verify Setup

### Test GitHub MCP (List Issues)
In a Copilot session, ask:
```
"List all open issues in the repository"
```

### Test Azure MCP (Check Functions)
In a Copilot session, ask:
```
"Show me the Azure Functions deployed to <function-app-name>"
```

### Test Playwright MCP
In a Copilot session, ask:
```
"Create a Playwright test that logs in and creates an auction"
```

## Troubleshooting

### "Authentication failed" from Azure MCP
- Verify Service Principal credentials in GitHub secrets
- Ensure Service Principal has Contributor role
- Check subscription ID is correct

### "Permission denied" for repository
- Check GitHub token has `repo`, `issues`, `workflow` scopes
- Verify Service Principal has correct resource access

### "Module not found" for Playwright
- Ensure `copilot-setup-steps.yml` runs successfully
- Check Node.js version is 18+
- Verify `npx playwright install` completed

## Next Steps

1. Commit these setup files to your main branch:
   ```bash
   git add .github/
   git commit -m "Configure MCP servers for Copilot"
   git push origin main
   ```

2. Verify `copilot-setup-steps.yml` runs successfully:
   - Go to **Actions** → **Copilot Setup Steps** → **Run workflow**
   - Confirm all steps pass

3. Test MCP servers in a Copilot session

4. Begin development with Copilot assistance!
