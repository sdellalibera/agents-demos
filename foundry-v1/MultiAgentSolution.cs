#:package Microsoft.Agents.AI.AzureAI.Persistent@1.0.0-preview.251125.1
#:package Azure.Identity@1.17.1

using Azure;
using Azure.Identity;
using Azure.AI.Agents.Persistent;
using Microsoft.Agents.AI;

var BingConnectionId = Environment.GetEnvironmentVariable("BingConnectionId");
var projectEndpoint = Environment.GetEnvironmentVariable("ProjectEndpoint");

var client = new PersistentAgentsClient(projectEndpoint, new DefaultAzureCredential());

BingGroundingToolDefinition bingGroundingTool = new(
    new BingGroundingSearchToolParameters(
        [new BingGroundingSearchConfiguration(BingConnectionId)]
    )
);

var openApiSpec = BinaryData.FromString(File.ReadAllText("../.openapi/weather_openapi.json"));
OpenApiToolDefinition openApiTool = new(
    new OpenApiFunctionDefinition("GetWeather", openApiSpec, new OpenApiAnonymousAuthDetails())
);

PersistentAgent BingAgent = await client.Administration.CreateAgentAsync(
    model: "gpt-4.1",
    name: "AgentWithGrounding",
    instructions: "You are an assistant used for retrieving actual informations",
    description: "Agent that retrieves actual information using Bing search",
    tools: [bingGroundingTool]
);

PersistentAgent WeatherAgent = await client.Administration.CreateAgentAsync(
    model: "gpt-4.1",
    name: "WeatherAgent",
    instructions: "You are an agent that returns informations about the weather",
    description: "Agent that provides weather information",
    tools: [openApiTool]
);

ConnectedAgentToolDefinition connectedBingAgent = new(
    new ConnectedAgentDetails(BingAgent.Id,BingAgent.Name,BingAgent.Description)
);

ConnectedAgentToolDefinition connectedWeatherAgent = new(
    new ConnectedAgentDetails(WeatherAgent.Id,WeatherAgent.Name,WeatherAgent.Description)
);

PersistentAgent OrchestratorAgent = await client.Administration.CreateAgentAsync(
    model: "gpt-4.1",
    name: "OrchestratorAgent",
    instructions: "You are a supervisor for different agents,redirect queries to the agent that best suits the request",
    tools: [connectedBingAgent,connectedWeatherAgent]
);

PersistentAgentThread thread = await client.Threads.CreateThreadAsync();

Console.WriteLine($"Created BingAgent: {BingAgent.Id}");
Console.WriteLine($"Created WeatherAgent: {WeatherAgent.Id}");
Console.WriteLine($"Created OrchestratorAgent: {OrchestratorAgent.Id}");
Console.WriteLine($"Created thread: {thread.Id}");

