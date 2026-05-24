#:package Microsoft.Agents.AI.AzureAI.Persistent@1.0.0-preview.251125.1
#:package Azure.AI.Projects@1.1.0
#:package Azure.Identity@1.17.1
#:package DotNetEnv@3.1.0

using Azure.Identity;
using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;

// Load .env — searches upward from the current directory, same as find_dotenv() in Python
DotNetEnv.Env.TraversePath().Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_FOUNDRY_ENDPOINT environment variable is not set.");

// Set AZURE_FOUNDRY_AGENT_ID to an existing agent ID.
// Create one with 01_persistent_agent.cs (comment out the deletion at the end) and copy the printed agent ID.
var agentId = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_AGENT_ID")
    ?? throw new InvalidOperationException("AZURE_FOUNDRY_AGENT_ID environment variable is not set.");

var projectClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());
PersistentAgentsClient agentClient = projectClient.GetPersistentAgentsClient();

// Retrieve the existing agent by ID — no need to recreate it
PersistentAgent agent = await agentClient.Administration.GetAgentAsync(agentId);
Console.WriteLine($"Using agent: {agent.Name} ({agent.Id})\n");

// --- First conversation ---
Console.WriteLine("=== First Conversation ===\n");
PersistentAgentThread thread1 = await agentClient.Threads.CreateThreadAsync();
await agentClient.Messages.CreateMessageAsync(thread1.Id, MessageRole.User, "Write a poem about the sea.");

await foreach (var update in agentClient.Runs.CreateRunStreamingAsync(thread1.Id, agent.Id))
{
    if (update is MessageContentUpdate content)
        Console.Write(content.Text);
}

// --- Second conversation (same agent, new thread) ---
Console.WriteLine("\n\n=== Second Conversation ===\n");
PersistentAgentThread thread2 = await agentClient.Threads.CreateThreadAsync();
await agentClient.Messages.CreateMessageAsync(thread2.Id, MessageRole.User, "Write a poem about the mountains.");

await foreach (var update in agentClient.Runs.CreateRunStreamingAsync(thread2.Id, agent.Id))
{
    if (update is MessageContentUpdate content)
        Console.Write(content.Text);
}

Console.WriteLine();
