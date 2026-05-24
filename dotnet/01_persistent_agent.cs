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

var projectClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());
PersistentAgentsClient agentClient = projectClient.GetPersistentAgentsClient();

// Create a persistent Foundry agent
PersistentAgent agent = await agentClient.Administration.CreateAgentAsync(
    model: "gpt-4.1",
    name: "WriterAgent",
    description: "An agent that writes creative stories",
    instructions: "You are a helpful agent that writes engaging book stories.");

Console.WriteLine($"Agent created: {agent.Id}");

// Create a conversation thread
PersistentAgentThread thread = await agentClient.Threads.CreateThreadAsync();
Console.WriteLine($"Thread created: {thread.Id}");

// Add a user message to the thread
await agentClient.Messages.CreateMessageAsync(
    thread.Id,
    MessageRole.User,
    "Write a short story about a robot who discovers music.");

// Stream the agent's response
Console.WriteLine("\n--- Agent Response ---\n");
await foreach (var update in agentClient.Runs.CreateRunStreamingAsync(thread.Id, agent.Id))
{
    if (update is MessageContentUpdate contentUpdate)
        Console.Write(contentUpdate.Text);
}

// Cleanup: delete the agent when done
await agentClient.Administration.DeleteAgentAsync(agent.Id);
Console.WriteLine("\n\nAgent deleted.");
