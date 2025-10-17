# 💬 OpenAI Chat App (.NET 10 + Blazor)

A simple and modern chat application built with **.NET 10** and **Blazor Server** that integrates with the **OpenAI API**.  
Talk naturally with AI through a clean, interactive interface featuring smooth auto-scroll, Enter-to-send, and Shift+Enter for new lines.

---

## 🚀 Features
- Built with **.NET 10** and **Blazor Server**
- Integrates with **OpenAI API** (or **Azure OpenAI**)
- Clean, modern UI with message bubbles
- Keyboard shortcuts: **Enter** to send, **Shift+Enter** for newline
- Auto-scrolls to latest message
- Easy setup and deployment

---

## 🧩 Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- A valid **OpenAI API key** or **Azure OpenAI** credentials

---

## ⚙️ Setup & Run
1. **Clone the repository:**
   ```bash
   git clone https://github.com/singhharvendra/OpenAIChat_Blazor_net10.git
   cd OpenAIChat_Blazor_net10.git
   
# Windows (PowerShell)
$env:OPENAI_API_KEY = "sk-your-key"
$env:OPENAI_MODEL = "gpt-4o-mini"

# macOS/Linux
export OPENAI_API_KEY="sk-your-key"
export OPENAI_MODEL="gpt-4o-mini"
dotnet restore
dotnet run --project src/OpenAIChat/OpenAIChat.csproj

# Example Prompt

“Tell me a fun fact about Blazor.”
“Summarize .NET 10’s new features in one paragraph.”

# Tech Stack

Blazor Server (.NET 10)
C#
OpenAI API / Azure OpenAI
HTML, CSS, JS for the chat interface
