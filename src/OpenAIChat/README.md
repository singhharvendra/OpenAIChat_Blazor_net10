# 💬 Blazor Chat App with OpenAI & Azure OpenAI (.NET 10)

A modern, conversational **chat application** built using **.NET 10 Blazor Server** that integrates seamlessly with both **OpenAI** and **Azure OpenAI** APIs.  
It provides a smooth, human-like chat experience through a clean interface, keyboard shortcuts, environment-based configuration, and full Docker support for easy deployment.

---

## ✨ Features

- ⚙️ Built with **.NET 10 + Blazor Server**
- 🤖 Works with both **OpenAI API** and **Azure OpenAI API**
- 💬 Natural, real-time AI chat with smooth auto-scroll
- ⌨️ Keyboard shortcuts (**Enter** = send, **Shift + Enter** = newline)
- 🧠 Lightweight `ChatService` that auto-detects environment (Azure / OpenAI)
- 🐳 Full **Docker support** for local and cloud deployment
- 🔒 Secure environment-variable configuration (no hard-coded secrets)

---

## 🧩 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- A valid **OpenAI API key** *or* an **Azure OpenAI** resource
- Docker (optional) if you plan to containerize or deploy

---

## 🔧 Configuration

### 🔹 Option 1 – Public OpenAI API

Create an API key at [https://platform.openai.com/account/api-keys](https://platform.openai.com/account/api-keys)  
Then set environment variables:

#### macOS / Linux
```bash
export OPENAI_API_KEY="sk-your-key"
export OPENAI_MODEL="gpt-4o-mini"
```

#### Windows PowerShell
```powershell
$env:OPENAI_API_KEY = "sk-your-key"
$env:OPENAI_MODEL   = "gpt-4o-mini"
```

> 💡 The app will automatically use these values when Azure variables aren’t set.

---

### 🔹 Option 2 – Azure OpenAI Integration

From your **Azure OpenAI** resource → **Keys and Endpoint**, copy:
- **Endpoint** (e.g., `https://your-resource.openai.azure.com`)
- **Key 1** or **Key 2**
- **Deployment Name** (e.g., `gpt-4o-mini`)
- **API Version** (e.g., `2024-08-01-preview`)

Then set:

#### macOS / Linux
```bash
export AZURE_OPENAI_ENDPOINT="https://<your-resource>.openai.azure.com"
export AZURE_OPENAI_API_KEY="<your-key>"
export AZURE_OPENAI_DEPLOYMENT="gpt-4o-mini"
export AZURE_OPENAI_API_VERSION="2024-08-01-preview"
```

#### Windows PowerShell
```powershell
$env:AZURE_OPENAI_ENDPOINT = "https://<your-resource>.openai.azure.com"
$env:AZURE_OPENAI_API_KEY  = "<your-key>"
$env:AZURE_OPENAI_DEPLOYMENT = "gpt-4o-mini"
$env:AZURE_OPENAI_API_VERSION = "2024-08-01-preview"
```

> 💡 Never commit secrets. `.env` and `appsettings.Development.json` are ignored in `.gitignore`.

---

## ▶️ Run Locally

```bash
dotnet build
dotnet run
```

Once it starts, open your browser at:  
👉 [http://localhost:5000](http://localhost:5000)

---

## 🐳 Docker Usage

### 🔨 Build the Docker Image

**Windows (PowerShell / CMD):**
```bash
docker build -f .\OpenAIChat_Blazor_net10\Dockerfile -t openaichatblazornet10:v1 .
```

**macOS / Linux (Terminal):**
```bash
docker build -f OpenAIChat_Blazor_net10/Dockerfile -t openaichatblazornet10:v1 .
```

---

### 🚀 Run the Docker Container
```bash
docker run -d -p 8080:8080 -p 8081:8081 --name openaichatblazornet10-v1 openaichatblazornet10:v1
```
Access at 👉 [http://localhost:8080](http://localhost:8080)

Alternate ports:
```bash
docker run -d -p 5000:8080 -p 5001:8081 --name openaichatblazornet10-v2 openaichatblazornet10:v2
```
Access at 👉 [http://localhost:5000](http://localhost:5000)

---

### 🧹 Docker Management Commands
```bash
docker ps                 # running containers
docker ps -a              # all containers
docker stop <id>          # stop container
docker rm <id>            # remove container

docker images             # list images
docker rmi <id>           # remove image
```

---

## ☁️ Deploy to Azure Web App (using Docker)

```bash

#Login and Create Resource Group

az login --use-device-code
az group create --name ai-chatbot --location eastus2

#Make sure you’re on the right subscription

az account set --subscription "<SUBSCRIPTION NAME OR ID>"

#Register required resource providers

# Azure OpenAI / Cognitive Services

az provider register --namespace Microsoft.CognitiveServices

# (Recommended) also register these once per subscription
az provider register --namespace Microsoft.Web                  # App Service
az provider register --namespace Microsoft.ContainerRegistry    # ACR


#(Optional) Azure OpenAI Resource and Model Deployment

# Create Azure OpenAI (Cognitive Services) resource

az cognitiveservices account create -n aoai-chat-1019 -g ai-chatbot -l eastus2 --kind OpenAI --sku S0 --custom-domain aoai-chat-1019 --yes

# Check available models

az cognitiveservices account list-models -g ai-chatbot -n aoai-chat-1019

# Deploy GPT model (Better to use portal if using free/standard tier)

az cognitiveservices account deployment create -g ai-chatbot -n aoai-chat-1019 --deployment-name o4-mini-1019 --model-name gpt-4.1 --model-version 2025-04-14 --model-format OpenAI

# Retrieve endpoint and key

az cognitiveservices account show -g ai-chatbot -n aoai-chat-1019 --query properties.endpoint -o tsv
az cognitiveservices account keys list -g ai-chatbot -n aoai-chat-1019 --query key1 -o tsv

#Create Azure Container Registry (ACR)

az acr create -g ai-chatbot -n openaichatacr1019 --sku Basic
az acr show -n openaichatacr1019 --query loginServer -o tsv


# Build Docker Image Locally

macOS/Linux

docker build -f src/OpenAIChat/Dockerfile -t openaichatblazornet10:v1 .

Windows

docker build -f src\OpenAIChat\Dockerfile -t openaichatblazornet10:v1 .

#Check Images and Remove Unused
docker images
docker rmi <image_id>

# Push Docker Image to ACR

az acr login -n openaichatacr1019
docker tag openaichatblazornet10:v1 openaichatacr1019.azurecr.io/openaichatblazornet10:v1
docker push openaichatacr1019.azurecr.io/openaichatblazornet10:v1

#Create App Service Plan and Web App (Linux)

az appservice plan create -g ai-chatbot -n chatapp-plan-1019 --sku F1 --is-linux

az webapp create -g ai-chatbot -p chatapp-plan -n chatapp-plan-1019 --deployment-container-image-name openaichatacr1019.azurecr.io/openaichatblazornet10:v1


```

Then open:  
👉 `https://openaichatdemo.azurewebsites.net`

---

## 🧠 Example Prompts
> “Explain the difference between Blazor Server and Blazor WebAssembly.”  
> “Summarize .NET 10’s new features in two sentences.”  
> “Write a short motivational quote about AI and innovation.”  
> “Act as a cloud architect and suggest Azure services for a chat app.”  

---

## 🧭 Troubleshooting

| Error | Cause | Fix |
|-------|--------|-----|
| `401 Unauthorized` | Missing / invalid API key | Verify `OPENAI_API_KEY` or `AZURE_OPENAI_API_KEY` |
| `404 Not Found` | Wrong endpoint or deployment name | Check Azure OpenAI resource |
| `429 Too Many Requests` | Quota or rate limit reached | Increase quota / reduce frequency |
| `insufficient_quota` | Account out of credits | Add billing info or request quota increase |
| Connection Timeout | Network blocked | Allow HTTPS outbound to OpenAI/Azure endpoint |

---

## 🛠️ Project Structure

```
OpenAIChat_Blazor_net10/
├── src/
│   └── OpenAIChat/
│       ├── Pages/Index.razor         # Chat UI
│       ├── Services/ChatService.cs   # Handles OpenAI & Azure OpenAI logic
│       ├── wwwroot/css/site.css      # Styles for chat interface
│       ├── wwwroot/js/site.js        # JS helpers (Enter-to-send, scroll)
│       ├── Program.cs                # App startup & DI configuration
│       ├── App.razor                 # Router & layout
│       ├── _Host.cshtml              # Blazor host page
│       └── Dockerfile                # Container build
└── README.md
```

---

## 🪄 Tech Stack
- **.NET 10 Blazor Server**
- **C# / Razor Components**
- **OpenAI / Azure OpenAI Chat Completions API**
- **HTML / CSS / JavaScript (Modern UI)**
- **Docker** for deployment