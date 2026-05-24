# Azure AI Foundry Agents — Demos

Code samples for building agents and multi-agent workflows with **Microsoft Foundry**, organized by language.

| Folder | Language | Format |
|--------|----------|--------|
| [`dotnet/`](dotnet/README.md) | C# / .NET 10 | Single-file scripts (`dotnet run`) |
| [`python/`](python/README.md) | Python 3.11+ | Jupyter notebooks |

## Samples covered

- **Persistent Foundry agent** — create an agent, open a thread, and stream the response
- **Multi-agent workflow** — chain a Planner agent into a Writer agent sequentially
- **Call an existing agent** — retrieve an agent by ID and run multiple independent conversations
- **Tool calls with Bing Grounding** — attach the Bing Search tool and observe live tool calls in the stream

## Quick links

- [.NET setup and samples →](dotnet/README.md)
- [Python setup and samples →](python/README.md)

## Authentication

All samples use `DefaultAzureCredential`. Run `az login` once and your local credentials are picked up automatically.
