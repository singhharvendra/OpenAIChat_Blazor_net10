# 💬 Blazor Chat App with OpenAI & Azure OpenAI (.NET 10)

A modern **Blazor Server** chat application built with **.NET 10**, integrating seamlessly with both **OpenAI** and **Azure OpenAI** APIs.  
It offers a real-time conversational experience through a clean, responsive interface — featuring keyboard shortcuts, secure configuration, and full Docker & Azure deployment support.

---

## ✨ Features

- ⚙️ Built using **.NET 10 + Blazor Server**
- 🤖 Compatible with both **OpenAI API** and **Azure OpenAI API**
- 💬 Real-time natural chat with smooth auto-scroll
- ⌨️ Keyboard shortcuts (**Enter** = send, **Shift + Enter** = newline)
- 🧠 Smart `ChatService` detects active environment automatically
- 🐳 Full **Docker support** for local and cloud deployment
- 🔒 Secure configuration using environment variables — no hard-coded secrets

---

## 🧩 Prerequisites

Before running or deploying the app, install the following:

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Git](https://git-scm.com/downloads)
- A valid **OpenAI API key** *or* an **Azure OpenAI** resource with a deployed model

---

## 🔧 Configuration

### Option 1 – Public OpenAI API

Generate an API key at [OpenAI API Keys](https://platform.openai.com/account/api-keys).

#### macOS / Linux
```bash
export OPENAI_API_KEY="sk-your-openai-key"
export OPENAI_MODEL="gpt-4o-mini"
```

#### Windows PowerShell
```powershell
$env:OPENAI_API_KEY = "sk-your-openai-key"
$env:OPENAI_MODEL   = "gpt-4o-mini"
```

> 💡 The app automatically falls back to OpenAI if Azure variables are not set.

---

### Option 2 – Azure OpenAI Integration

Obtain details from your Azure OpenAI resource:

- **Endpoint** → e.g. `https://<your-resource>.openai.azure.com`
- **API Key**
- **Deployment Name** → e.g. `gpt-4o-mini`
- **API Version** → e.g. `2024-08-01-preview`

#### macOS / Linux
```bash
export AZURE_OPENAI_ENDPOINT="https://<your-resource>.openai.azure.com"
export AZURE_OPENAI_API_KEY="<your-azure-api-key>"
export AZURE_OPENAI_DEPLOYMENT="gpt-4o-mini"
export AZURE_OPENAI_API_VERSION="2024-08-01-preview"
```

#### Windows PowerShell
```powershell
$env:AZURE_OPENAI_ENDPOINT = "https://<your-resource>.openai.azure.com"
$env:AZURE_OPENAI_API_KEY  = "<your-azure-api-key>"
$env:AZURE_OPENAI_DEPLOYMENT = "gpt-4o-mini"
$env:AZURE_OPENAI_API_VERSION = "2024-08-01-preview"
```

> ⚠️ Do **not** commit secrets — ensure `.env` and `appsettings.Development.json` are in `.gitignore`.

---

## ▶️ Run Locally

```bash
dotnet build
dotnet run
```

Then open your browser:  
👉 [http://localhost:5000](http://localhost:5000)

---

## 🐳 Docker Usage

### Build Docker Image

**Windows (PowerShell / CMD):**
```bash
docker build -f .\OpenAIChat_Blazor_net10\Dockerfile -t blazorchatai:v1 .
```

**macOS / Linux:**
```bash
docker build -f OpenAIChat_Blazor_net10/Dockerfile -t blazorchatai:v1 .
```

---

### Run Docker Container
```bash
docker run -d -p 8080:8080 -p 8081:8081 --name blazorchatai-v1 blazorchatai:v1
```
Access at 👉 [http://localhost:8080](http://localhost:8080)

Alternate ports:
```bash
docker run -d -p 5000:8080 -p 5001:8081 --name blazorchatai-v2 blazorchatai:v1
```
Access at 👉 [http://localhost:5000](http://localhost:5000)

---

### Docker Management Commands

```bash
docker ps                 # List running containers
docker ps -a              # List all containers
docker stop <container_id> # Stop a container
docker rm <container_id>   # Remove a container

docker images             # List Docker images
docker rmi <image_id>     # Remove Docker image
```

---

## ☁️ Deploy to Azure (Docker + ACR + App Service)

### 1️⃣ Login & Create Resource Group

```bash
az login --use-device-code
az group create --name <YOUR_RG_NAME> --location eastus2
```

> Replace `<YOUR_RG_NAME>` with your preferred resource group name (e.g., `ai-chatbot`).

---

### 2️⃣ Register Providers

```bash
az provider register --namespace Microsoft.CognitiveServices
az provider register --namespace Microsoft.Web
az provider register --namespace Microsoft.ContainerRegistry
```

---

### 3️⃣ Create Azure OpenAI Resource (Optional)

```bash
az cognitiveservices account create -n <YOUR_OPENAI_RESOURCE> -g <YOUR_RG_NAME> -l eastus2 --kind OpenAI --sku S0 --custom-domain <YOUR_OPENAI_RESOURCE> --yes
az cognitiveservices account deployment create -g <YOUR_RG_NAME> -n <YOUR_OPENAI_RESOURCE> --deployment-name <YOUR_DEPLOYMENT_NAME> --model-name gpt-4o-mini --model-version 2024-07-18 --model-format OpenAI
```

Retrieve endpoint and key:
```bash
az cognitiveservices account show -g <YOUR_RG_NAME> -n <YOUR_OPENAI_RESOURCE> --query properties.endpoint -o tsv
az cognitiveservices account keys list -g <YOUR_RG_NAME> -n <YOUR_OPENAI_RESOURCE> --query key1 -o tsv
```

---

### 4️⃣ Create Azure Container Registry (ACR)

```bash
az acr create -g <YOUR_RG_NAME> -n <YOUR_ACR_NAME> --sku Basic
az acr show -n <YOUR_ACR_NAME> --query loginServer -o tsv
```

---

### 5️⃣ Build and Push Docker Image

**Build (Windows):**
```bash
docker build -f src\OpenAIChat\Dockerfile -t blazorchatai:v1 .
```

**Build (macOS/Linux):**
```bash
docker build -f src/OpenAIChat/Dockerfile -t blazorchatai:v1 .
```

**Push to ACR:**
```bash
az acr login -n <YOUR_ACR_NAME>
docker tag blazorchatai:v1 <YOUR_ACR_NAME>.azurecr.io/blazorchatai:v1
docker push <YOUR_ACR_NAME>.azurecr.io/blazorchatai:v1
```

---

### 6️⃣ Create App Service Plan & Web App

```bash
az appservice plan create -g <YOUR_RG_NAME> -n <YOUR_PLAN_NAME> --sku B1 --is-linux
az webapp create -g <YOUR_RG_NAME> -p <YOUR_PLAN_NAME> -n <YOUR_APP_NAME> --deployment-container-image-name <YOUR_ACR_NAME>.azurecr.io/blazorchatai:v1
```

---

### 7️⃣ Assign Permissions & Configure ACR Pull Access

```bash
az webapp identity assign -g <YOUR_RG_NAME> -n <YOUR_APP_NAME>
PRINCIPAL_ID=$(az webapp identity show -g <YOUR_RG_NAME> -n <YOUR_APP_NAME> --query principalId -o tsv)
ACR_ID=$(az acr show -n <YOUR_ACR_NAME> -g <YOUR_RG_NAME> --query id -o tsv)
az role assignment create --assignee "$PRINCIPAL_ID" --role "AcrPull" --scope "$ACR_ID"
```

---

### 8️⃣ Configure Web App Container Settings

```bash
az webapp config container set --resource-group <YOUR_RG_NAME> --name <YOUR_APP_NAME> --docker-custom-image-name <YOUR_ACR_NAME>.azurecr.io/blazorchatai:v1 --docker-registry-server-url "https://<YOUR_ACR_NAME>.azurecr.io"
```

---

### 9️⃣ Add App Settings (Keys and Config)

```bash
az webapp config appsettings set -g <YOUR_RG_NAME> -n <YOUR_APP_NAME> --settings AZURE_OPENAI_ENDPOINT="https://<YOUR_OPENAI_RESOURCE>.openai.azure.com" AZURE_OPENAI_API_KEY="<YOUR_API_KEY>" AZURE_OPENAI_DEPLOYMENT="gpt-4o-mini" AZURE_OPENAI_API_VERSION="2024-08-01-preview"
```

---

### 🔗 Access the App

Once deployed, visit:
```
https://<YOUR_APP_NAME>.azurewebsites.net
```

---

## 🧠 Example Prompts

> “Explain the difference between Blazor Server and Blazor WebAssembly.”  
> “Summarize .NET 10’s new features in two sentences.”  
> “Write a motivational quote about AI and innovation.”  
> “Suggest Azure services for a chat app as a cloud architect.”  

---

## 🧭 Troubleshooting

| Error | Cause | Fix |
|-------|--------|-----|
| `401 Unauthorized` | Invalid API key | Check `OPENAI_API_KEY` / `AZURE_OPENAI_API_KEY` |
| `404 Not Found` | Wrong endpoint or deployment | Verify Azure OpenAI deployment name |
| `429 Too Many Requests` | Quota exceeded | Reduce frequency or request quota increase |
| `insufficient_quota` | Billing issue | Add payment or request more quota |
| `Timeout` | Network issue | Ensure outbound HTTPS access |

---

## 🛠️ Project Structure

```
OpenAIChat_Blazor_net10/
├── src/
│   └── OpenAIChat/
│       ├── Pages/Index.razor
│       ├── Services/ChatService.cs
│       ├── wwwroot/css/site.css
│       ├── wwwroot/js/site.js
│       ├── Program.cs
│       ├── App.razor
│       ├── _Host.cshtml
│       └── Dockerfile
└── README.md
```

---

## 🪄 Tech Stack

- **.NET 10 Blazor Server**
- **C# / Razor Components**
- **OpenAI / Azure OpenAI Chat API**
- **HTML / CSS / JavaScript**
- **Docker + Azure Web App (Linux)**

---

## 📄 License

MIT License © 2025 Harvendra Singh
