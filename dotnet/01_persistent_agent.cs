#:package Microsoft.Agents.AI.Foundry@1.5.0
#:package Microsoft.Agents.AI@1.5.0
#:package Azure.AI.Projects@2.0.1
#:package Azure.Identity@1.17.1
#:package DotNetEnv@3.1.0

using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;

// Load .env — searches upward from the current directory, same as find_dotenv() in Python
DotNetEnv.Env.TraversePath().Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_FOUNDRY_ENDPOINT environment variable is not set.");

var projectClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());

// Create a persistent Foundry v2 (Prompt) agent via the Microsoft Agent Framework adapter.
// AsAIAgent creates a new agent version under the given name and returns an invocable AIAgent.
const string AgentName = "WriterAgent";
AIAgent agent = projectClient.AsAIAgent(
    model: "gpt-4.1",
    instructions: "You are a helpful agent that writes engaging book stories.",
    name: AgentName);

Console.WriteLine($"Agent created: {agent.Name} ({agent.Id})");

// Stream the agent's response — v2 has no threads/runs; MAF handles the Responses API.
Console.WriteLine("\n--- Agent Response ---\n");
await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync(
    "Write a short story about a robot who discovers music."))
{
    Console.Write(update.Text);
}

// Cleanup: delete version 1 of the agent we just created.
await projectClient.AgentAdministrationClient.DeleteAgentVersionAsync(agentName: AgentName, agentVersion: "1");
Console.WriteLine("\n\nAgent deleted.");
