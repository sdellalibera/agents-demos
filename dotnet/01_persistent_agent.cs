#:package Microsoft.Agents.AI.Foundry@1.5.0
#:package Microsoft.Agents.AI@1.5.0
#:package Azure.AI.Projects@2.0.1
#:package Azure.Identity@1.21.0
#:package DotNetEnv@3.1.0
#:project Shared/Shared.csproj

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

var projectClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());

ConsoleHelpers.Pause("create the persistent agent");

// Create a persistent Foundry v2 (Prompt) agent version
const string AgentName = "WriterAgent";
var definition = new DeclarativeAgentDefinition(model: "gpt-4.1")
{
    Instructions = "You are a helpful agent that writes engaging book stories.",
};
ProjectsAgentVersion version = await projectClient.AgentAdministrationClient.CreateAgentVersionAsync(
    agentName: AgentName,
    options: new ProjectsAgentVersionCreationOptions(definition));

Console.WriteLine($"Agent created: {version.Name} (version {version.Version})");

ConsoleHelpers.Pause("stream the agent's response");

// Wrap the persistent agent version as a Microsoft Agent Framework AIAgent and stream a response
AIAgent agent = projectClient.AsAIAgent(version);

Console.WriteLine("\n--- Agent Response ---\n");
await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(
    "Write a short story about a robot who discovers music."))
{
    Console.Write(update.Text);
}

ConsoleHelpers.Pause("delete the agent");

// Cleanup: delete the agent (all versions) when done
await projectClient.AgentAdministrationClient.DeleteAgentAsync(AgentName);
Console.WriteLine("\nAgent deleted.");
