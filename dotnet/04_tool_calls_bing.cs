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

// Set BING_CONNECTION_ID to your Bing Grounding connection ID from the Azure AI Foundry portal
var bingConnectionId = Environment.GetEnvironmentVariable("BING_CONNECTION_ID")
    ?? throw new InvalidOperationException("BING_CONNECTION_ID environment variable is not set.");

var projectClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());
PersistentAgentsClient agentClient = projectClient.GetPersistentAgentsClient();

// Define the Bing Grounding tool
BingGroundingToolDefinition bingTool = new BingGroundingToolDefinition(
    new BingGroundingSearchToolParameters(
        searchConfigurations: [new BingGroundingSearchConfiguration(bingConnectionId)]
    )
);

// Create an agent with Bing Grounding enabled
PersistentAgent agent = await agentClient.Administration.CreateAgentAsync(
    model: "gpt-4.1",
    name: "GroundedSearchAgent",
    instructions: "You answer questions using Bing search to find current, accurate information.",
    tools: [bingTool]);

Console.WriteLine($"Agent created: {agent.Id}");

// Ask a question that requires live information
PersistentAgentThread thread = await agentClient.Threads.CreateThreadAsync();
await agentClient.Messages.CreateMessageAsync(
    thread.Id,
    MessageRole.User,
    "What are the latest major developments in AI in the past month?");

// Stream the agent's response
Console.WriteLine("\n--- Agent Response ---\n");

await foreach (var update in agentClient.Runs.CreateRunStreamingAsync(thread.Id, agent.Id))
{
    if (update is MessageContentUpdate contentUpdate)
        Console.Write(contentUpdate.Text);
}

// Cleanup
await agentClient.Administration.DeleteAgentAsync(agent.Id);
Console.WriteLine("\nAgent deleted.");
