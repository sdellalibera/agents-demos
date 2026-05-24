#:package Microsoft.Agents.AI.Foundry@1.5.0
#:package Microsoft.Agents.AI@1.5.0
#:package Azure.AI.Projects@2.0.1
#:package Azure.Identity@1.21.0
#:package DotNetEnv@3.1.0
#:project Shared/Shared.csproj

using System.ClientModel;
using AgentsDemos;
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// Load .env — searches upward from the current directory, same as find_dotenv() in Python
DotNetEnv.Env.TraversePath().Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_FOUNDRY_ENDPOINT environment variable is not set.");

// Set these to the name and version printed by 01_persistent_agent.cs
const string AgentName = "WriterAgent";
const string AgentVersion = "1";

var projectClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());
var admin = projectClient.AgentAdministrationClient;

ConsoleHelpers.Pause($"get or create agent '{AgentName}'");

// Try to retrieve the existing agent version; if it doesn't exist (e.g. it was
// deleted by a previous sample), create it on the fly so this script still works.
ProjectsAgentVersion version;
try
{
    version = await admin.GetAgentVersionAsync(agentName: AgentName, agentVersion: AgentVersion);
    Console.WriteLine($"Found existing agent: {version.Name} (version {version.Version})");
}
catch (ClientResultException ex) when (ex.Status == 404)
{
    Console.WriteLine($"Agent '{AgentName}' v{AgentVersion} not found — creating it now...");
    version = await admin.CreateAgentVersionAsync(
        agentName: AgentName,
        options: new ProjectsAgentVersionCreationOptions(new DeclarativeAgentDefinition("gpt-4.1")
        {
            Instructions = "You are a helpful agent that writes engaging book stories.",
        }));
    Console.WriteLine($"Agent created: {version.Name} (version {version.Version})");
}

AIAgent agent = projectClient.AsAIAgent(version);

// Each AIAgent.RunStreamingAsync call is a stateless conversation — same agent serves multiple

ConsoleHelpers.Pause("run the first conversation");

Console.WriteLine("\n=== First Conversation ===\n");
await foreach (AgentResponseUpdate update in agent.RunStreamingAsync("Write a poem about the sea."))
{
    Console.Write(update.Text);
}

ConsoleHelpers.Pause("run the second conversation");

Console.WriteLine("\n=== Second Conversation ===\n");
await foreach (AgentResponseUpdate update in agent.RunStreamingAsync("Write a poem about the mountains."))
{
    Console.Write(update.Text);
}

Console.WriteLine();
