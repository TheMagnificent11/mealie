# Deployment Guide

This document describes how to deploy the Mealie Aspire application to Azure using the Continuous Deployment (CD) workflow.

## Prerequisites

### Azure Resources

The deployment workflow requires an Azure subscription and a configured Microsoft Entra ID (formerly Azure AD) App Registration with appropriate permissions.

### Required Azure Secrets

The following secrets must be configured in the GitHub repository settings (Settings → Secrets and variables → Actions):

| Secret Name | Description |
|-------------|-------------|
| `AZURE__TENANTID` | Your Azure tenant ID |
| `AZURE__CLIENTID` | The Application (client) ID from your App Registration |
| `AZURE__SUBSCRIPTIONID` | Your Azure subscription ID |
| `AZURE__RESOURCEGROUP` | The name of the Azure resource group for deployment |
| `AZURE__LOCATION` | The Azure region (e.g., `eastus`, `westeurope`) |

**Note:** `AZURE__CLIENTSECRET` is not used because the workflow uses OpenID Connect (OIDC) federated identity credentials instead of client secrets.

## Azure App Registration Configuration

### Step 1: Create an App Registration

1. Navigate to the [Azure Portal](https://portal.azure.com)
2. Go to **Microsoft Entra ID** → **App registrations**
3. Click **New registration**
4. Enter a name (e.g., `github-deploy`)
5. Select **Accounts in this organizational directory only**
6. Click **Register**

### Step 2: Configure Federated Identity Credentials

The deployment workflow uses OIDC authentication, which requires configuring a federated identity credential:

1. In your App Registration, navigate to **Certificates & secrets**
2. Click on the **Federated credentials** tab
3. Click **Add credential**
4. Select **GitHub Actions deploying Azure resources**
5. Fill in the following details:
   - **Organization**: `TheMagnificent11`
   - **Repository**: `mealie`
   - **Entity type**: `Branch`
   - **GitHub branch name**: `main`
   - **Name**: `github-deploy-main` (or any descriptive name)
6. Click **Add**

The federated credential will have the following subject claim:
```
repo:TheMagnificent11/mealie:ref:refs/heads/main
```

### Step 3: Grant Azure Permissions

The App Registration needs permissions to deploy resources to your Azure subscription:

1. Navigate to your Azure **Subscription**
2. Click **Access control (IAM)**
3. Click **Add** → **Add role assignment**
4. Select the **Contributor** role (or a more restrictive custom role with deployment permissions)
5. Click **Next**
6. Select **User, group, or service principal**
7. Click **Select members**
8. Search for your App Registration name (e.g., `github-deploy`)
9. Select it and click **Select**
10. Click **Review + assign**

## Deployment Workflow

The CD workflow (`.github/workflows/cd.yml`) is triggered:
- Automatically when the CI workflow completes successfully on the `main` branch
- Manually via workflow dispatch in the GitHub Actions UI

### Workflow Steps

1. **Checkout** - Clones the repository
2. **Setup .NET** - Installs .NET 10.0 SDK
3. **Install Aspire Workload** - Installs the Aspire CLI tools
4. **Azure Login** - Authenticates to Azure using OIDC federated credentials
5. **Build** - Builds the solution in Release configuration
6. **Deploy Application** - Deploys the Aspire application to Azure using `aspire deploy`

### Aspire Deploy Configuration

The `aspire deploy` command reads configuration from environment variables and the `.aspire/settings.json` file:

- **Environment variables** (from GitHub secrets):
  - `AZURE__SUBSCRIPTIONID`
  - `AZURE__RESOURCEGROUP`
  - `AZURE__LOCATION`
- **App Host path**: Configured in `.aspire/settings.json`

## Troubleshooting

### Error: "has no configured federated identity credentials"

**Full error message:**
```
AADSTS70025: The client '***'(github-deploy) has no configured federated identity credentials.
```

**Solution:**
This error occurs when the Azure App Registration does not have a federated identity credential configured for GitHub Actions OIDC. Follow **Step 2: Configure Federated Identity Credentials** above to resolve this issue.

### Error: "auth-type is incorrect"

**Solution:**
Ensure that you have configured a federated identity credential in Azure (not a client secret). The workflow uses OIDC authentication and does not require `AZURE__CLIENTSECRET`.

### Error: "Insufficient permissions"

**Solution:**
Verify that the App Registration has been granted the **Contributor** role (or appropriate permissions) on the target Azure subscription or resource group. See **Step 3: Grant Azure Permissions** above.

## Manual Deployment

To deploy manually from your local machine:

1. Ensure you have the Azure CLI installed and are logged in:
   ```bash
   az login
   ```

2. Set the required environment variables:
   ```bash
   export AZURE__SUBSCRIPTIONID="your-subscription-id"
   export AZURE__RESOURCEGROUP="your-resource-group"
   export AZURE__LOCATION="eastus"
   ```

3. Build the application:
   ```bash
   dotnet build --configuration Release --nologo
   ```

4. Deploy using Aspire CLI:
   ```bash
   aspire deploy --environment Production --clear-cache --include-exception-details
   ```

## Security Considerations

- **Use OIDC over client secrets**: The workflow is configured to use OIDC federated identity credentials, which are more secure than long-lived client secrets
- **Principle of least privilege**: Grant only the minimum permissions required for deployment
- **Protect secrets**: Ensure GitHub repository secrets are only accessible to authorized collaborators
- **Review federated credentials**: Periodically review and remove unused federated credentials from your App Registration
