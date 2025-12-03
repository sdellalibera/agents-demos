
#:package Microsoft.Agents.AI.AzureAI.Persistent@1.0.0-preview.251125.1
#:package Azure.Identity@1.17.1

using Azure;
using Azure.Identity;
using Azure.AI.Agents.Persistent;

var BingConnectionId = "/subscriptions/ce579f0b-d3a2-403b-aabc-5d162210094e/resourceGroups/rg-aspire-agent-service/providers/Microsoft.CognitiveServices/accounts/foundry-agentservice/projects/agentservice/connections/binggroundingsearchagentservice";

BingGroundingToolDefinition bingGroundingTool = new(
    new BingGroundingSearchToolParameters(
        [new BingGroundingSearchConfiguration(BingConnectionId)]
    )
);

var projectEndpoint = "https://foundry-agentservice.services.ai.azure.com/api/projects/agentservice";
var client = new PersistentAgentsClient(projectEndpoint, new DefaultAzureCredential());

PersistentAgent agent = await client.Administration.CreateAgentAsync(
    model: "gpt-4.1",
    name: "AgentWithGrounding",
    instructions: "You are an assistant used for retrieving actual informations",
    tools: [bingGroundingTool]
);

PersistentAgentThread thread = await client.Threads.CreateThreadAsync();

Console.WriteLine($"Created GroundingAgent: {agent.Id}");

Console.WriteLine($"Created thread: {thread.Id}");

