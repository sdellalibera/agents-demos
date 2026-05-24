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

var bingConnectionName = Environment.GetEnvironmentVariable("BING_CONNECTION_NAME")
    ?? throw new InvalidOperationException("BING_CONNECTION_NAME environment variable is not set.");

var projectClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());

ConsoleHelpers.Pause("resolve the Bing Grounding connection");

// Resolve the Bing Grounding connection ID by name from the project's connections
AIProjectConnection bingConnection = await projectClient.Connections.GetConnectionAsync(
    connectionName: bingConnectionName, includeCredentials: false);
string bingConnectionId = bingConnection.Id;
Console.WriteLine($"Bing connection: {bingConnection.Name}");

ConsoleHelpers.Pause("create the Bing-grounded agent");

// Define the Bing Grounding tool and create a persistent Foundry v2 (Prompt) agent
var bingTool = new BingGroundingTool(new BingGroundingSearchToolOptions(
    searchConfigurations: new[] { new BingGroundingSearchConfiguration(bingConnectionId) }));

const string AgentName = "GroundedSearchAgent";
var definition = new DeclarativeAgentDefinition(model: "gpt-4.1")
{
    Instructions = "You answer questions using Bing search to find current, accurate information.",
};
definition.Tools.Add(bingTool);

ProjectsAgentVersion version = await projectClient.AgentAdministrationClient.CreateAgentVersionAsync(
    agentName: AgentName,
    options: new ProjectsAgentVersionCreationOptions(definition));
Console.WriteLine($"Agent created: {version.Name} (version {version.Version})");

AIAgent agent = projectClient.AsAIAgent(version);

ConsoleHelpers.Pause("ask the agent a question grounded by Bing");

// Ask a question that requires live information — stream the agent's response
Console.WriteLine("\n--- Agent Response ---\n");
await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(
    "Who is the president of the United States?"))
{
    Console.Write(update.Text);
}

ConsoleHelpers.Pause("delete the agent");

// Cleanup: delete the agent (all versions) when done
await projectClient.AgentAdministrationClient.DeleteAgentAsync(AgentName);
Console.WriteLine("\nAgent deleted.");
