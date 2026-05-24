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
var admin = projectClient.AgentAdministrationClient;

ConsoleHelpers.Pause("create the Planner and Writer agents");

// Create two specialized persistent Foundry v2 (Prompt) agents
ProjectsAgentVersion plannerVersion = await admin.CreateAgentVersionAsync(
    agentName: "PlannerAgent",
    options: new ProjectsAgentVersionCreationOptions(new DeclarativeAgentDefinition("gpt-4.1")
    {
        Instructions = "You create concise story outlines with 3-4 bullet points.",
    }));

ProjectsAgentVersion writerVersion = await admin.CreateAgentVersionAsync(
    agentName: "WriterAgent",
    options: new ProjectsAgentVersionCreationOptions(new DeclarativeAgentDefinition("gpt-4.1")
    {
        Instructions = "You write short, engaging stories (3-4 paragraphs) based on outlines.",
    }));

Console.WriteLine($"PlannerAgent: {plannerVersion.Name} (version {plannerVersion.Version})");
Console.WriteLine($"WriterAgent:  {writerVersion.Name} (version {writerVersion.Version})");

AIAgent planner = projectClient.AsAIAgent(plannerVersion);
AIAgent writer = projectClient.AsAIAgent(writerVersion);

ConsoleHelpers.Pause("run the Planner agent");

// Step 1 — Planner creates the story outline
Console.WriteLine("\n=== PlannerAgent: Creating Story Outline ===\n");
AgentResponse plannerResponse = await planner.RunAsync(
    "Create an outline for a story about an AI that learns to paint.");
string outline = plannerResponse.Text;
Console.WriteLine(outline);

ConsoleHelpers.Pause("run the Writer agent on the outline");

// Step 2 — Writer expands the outline into a full story (streaming)
Console.WriteLine("\n=== WriterAgent: Writing the Story ===\n");
await foreach (AgentResponseUpdate update in writer.RunStreamingAsync(
    $"Write a story based on this outline:\n{outline}"))
{
    Console.Write(update.Text);
}

ConsoleHelpers.Pause("delete both agents");

// Cleanup: delete both agents (all versions)
await admin.DeleteAgentAsync("PlannerAgent");
await admin.DeleteAgentAsync("WriterAgent");
Console.WriteLine("\nBoth agents deleted.");
