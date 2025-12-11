#:package Microsoft.Agents.AI.AzureAI.Persistent@1.0.0-preview.251125.1
//#:package Microsoft.Agents.AI@1.0.0-preview.251204.1
//#:package Microsoft.Extensions.AI@10.1.0
#:package Azure.AI.Projects@1.1.0
#:package Azure.Identity@1.17.1

using Azure.Identity;
using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;

//set project endpoint
const string endpoint = "https://sdellalibera-foundry-demos.services.ai.azure.com/api/projects/foundry-demos";

//declare AIProjectClient
var AIProjectClient = new AIProjectClient(new Uri(endpoint),new DefaultAzureCredential());

//declare PersistentAgentsClient
PersistentAgentsClient agentClient = AIProjectClient.GetPersistentAgentsClient();

//define new agent
PersistentAgent agent = await agentClient.Administration.CreateAgentAsync(
    model: "gpt-4.1",
    name: "WriterAgent",
    description: "This agent writes new stories",
    instructions: "You are an helpful agent that writes book stories"
);
Console.WriteLine($"Agent created: {agent.Id}");

PersistentAgentThread thread = await agentClient.Threads.CreateThreadAsync();
Console.WriteLine($"Thread created: {thread.Id}");

PersistentThreadMessage message = await agentClient.Messages.CreateMessageAsync(
    thread.Id,
    MessageRole.User,
    "Write a new interesting story"
);

//create streaming run
var streamingRun = agentClient.Runs.CreateRunStreamingAsync(thread.Id, agent.Id);

await foreach (var chunk in streamingRun)
{
    if (chunk is MessageContentUpdate contentUpdate)
    {
        Console.Write(contentUpdate.Text);
    }
}

var agentDelete = await agentClient.Administration.DeleteAgentAsync(agent.Id);
Console.WriteLine($"\n\nDeleted agent:{agentDelete.Value}");


