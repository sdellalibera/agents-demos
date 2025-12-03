#:package Azure.AI.Agents.Persistent@1.2.0-beta.7
#:package Azure.Identity@1.17.1

using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;


var projectEndpoint = "https://foundry-agentservice.services.ai.azure.com/api/projects/agentservice";
var client = new PersistentAgentsClient(projectEndpoint, new DefaultAzureCredential());

var openApiSpec = BinaryData.FromString(File.ReadAllText("../.openapi/weather_openapi.json"));
OpenApiToolDefinition openApiTool = new(
    new OpenApiFunctionDefinition("GetWeather", openApiSpec, new OpenApiAnonymousAuthDetails())
);

PersistentAgent agent = await client.Administration.CreateAgentAsync(
    model: "gpt-4.1",
    name: "WeatherAgent",
    instructions: "You are an agent that returns informations about the weather",
    tools: [openApiTool]
);

PersistentAgentThread thread = await client.Threads.CreateThreadAsync();

Console.WriteLine($"Created WeatherAgent: {agent.Id}");
Console.WriteLine($"Created thread: {thread.Id}");

