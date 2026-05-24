# .NET Samples — Azure AI Foundry Agents

These samples use [.NET file-based programs](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/sdk#file-based-programs) (introduced in .NET 10), which allow running a single `.cs` file directly with `dotnet run`. Dependencies are declared with `#:package` directives at the top of each file — no project file needed.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- An [Azure AI Foundry](https://ai.azure.com) project with a deployed `gpt-4.1` model
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) installed and signed in (`az login`)

## Setup

Copy the sample env file at the repo root and fill in your values:

```bash
cp .env.sample .env
# then edit .env with your endpoint and connection details
```

Each script loads `.env` automatically from the repo root — no `export` needed.

> **`AZURE_FOUNDRY_AGENT_ID`** — needed only for `03_call_agent.cs`. Run `01_persistent_agent.cs` first with the cleanup lines commented out and copy the printed agent ID.
>
> **`BING_CONNECTION_ID`** — needed only for `04_tool_calls_bing.cs`. Create a Bing Search connection in the Foundry portal and copy its resource ID.

## Samples

| File | Description |
|------|-------------|
| `01_persistent_agent.cs` | Create a persistent Foundry agent and stream its response |
| `02_agent_workflow.cs` | Multi-agent workflow — Planner chains into Writer using MEAI `IChatClient` |
| `03_call_agent.cs` | Call an existing persistent agent by ID across multiple conversation threads |
| `04_tool_calls_bing.cs` | Persistent agent with Bing Grounding tool — observe tool calls in the stream |

## Running a Sample

```bash
dotnet run dotnet/01_persistent_agent.cs
```

Each file is self-contained. Authentication uses `DefaultAzureCredential`, which picks up your `az login` session automatically.
