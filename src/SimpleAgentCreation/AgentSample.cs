#:package Azure.AI.Agents.Persistent@1.2.0-beta.7
#:package Azure.Identity@1.17.1

using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;

var projectEndpoint = "https://foundry-agentservice.services.ai.azure.com/api/projects/agentservice";
var client = new PersistentAgentsClient(projectEndpoint, new DefaultAzureCredential());

PersistentAgent agent = await client.Administration.CreateAgentAsync(
    model: "gpt-4.1",
    name: "BaseAgent",
    instructions: "You are an helpful assistant"
);

PersistentAgentThread thread = await client.Threads.CreateThreadAsync();

Console.WriteLine($"Created agent: {agent.Id}");
Console.WriteLine($"Created thread: {thread.Id}");

