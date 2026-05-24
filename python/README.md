# Python Samples — Azure AI Foundry Agents

These samples use [Jupyter notebooks](https://jupyter.org/) with the `azure-ai-projects` Python SDK. Each notebook is self-contained: the first cell installs the required packages, so no manual setup is needed beyond the environment variables below.

## Prerequisites

- Python 3.11 or later
- An [Azure AI Foundry](https://ai.azure.com) project with a deployed `gpt-4.1` model
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) installed and signed in (`az login`)
- VS Code with the [Jupyter extension](https://marketplace.visualstudio.com/items?itemName=ms-toolsai.jupyter) (or any other Jupyter-compatible environment)

## Setup

### 1. Create a virtual environment

From the repo root:

```bash
cd python
python3 -m venv .venv
source .venv/bin/activate          # Windows: .venv\Scripts\activate
pip install -r requirements.txt
python -m ipykernel install --user --name agents-demos-py --display-name "Python (agents-demos)"
```

> Attendees: re-run `source .venv/bin/activate` in every new terminal. The `.venv/` folder is gitignored and is meant to be recreated per machine. The `ipykernel install` step registers a kernel named **Python (agents-demos)** so VS Code lists it in the kernel picker without having to browse to the venv binary.

### 2. Configure environment variables

Copy the sample env file at the repo root and fill in your values:

```bash
cp ../.env.sample ../.env
# then edit .env with your endpoint and connection details
```

Each notebook calls `load_dotenv(find_dotenv())` in its setup cell, which locates and loads `.env` from the repo root automatically.

### 3. Select the kernel in VS Code

Open any notebook, click the kernel picker in the top-right, and choose **Python (agents-demos)**. If you skipped the `ipykernel install` step, pick **Python Environments → .venv** instead.

> **`AZURE_FOUNDRY_AGENT_ID`** — needed only for `03_call_agent.ipynb`. Run `01_persistent_agent.ipynb` first (skip the cleanup cell) and copy the printed agent ID.
>
> **`BING_CONNECTION_ID`** — needed only for `04_tool_calls_bing.ipynb`. Create a Bing Search connection in the Foundry portal and copy its resource ID.

## Samples

| Notebook | Description |
|----------|-------------|
| `01_persistent_agent.ipynb` | Create a persistent Foundry agent and stream its response |
| `02_agent_workflow.ipynb` | Multi-agent workflow — PlannerAgent feeds output into WriterAgent |
| `03_call_agent.ipynb` | Call an existing persistent agent by ID across multiple conversation threads |
| `04_tool_calls_bing.ipynb` | Persistent agent with Bing Grounding tool — observe tool calls in the stream |

## Running the Notebooks

Open any notebook in VS Code, select a Python kernel, and run cells top to bottom. The first cell in each notebook installs all required packages via `%pip install`.

Authentication uses `DefaultAzureCredential`, which picks up your `az login` session automatically.
