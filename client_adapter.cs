#:package Azure.Identity@1.17.1
#:package Azure.AI.OpenAI@2.1.0
#:package Microsoft.Agents.AI@1.0.0-preview.251204.1
#:package Microsoft.Extensions.AI.OpenAI@10.1.0-preview.1.25608.1

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

const string endpoint = "https://sdellalibera-foundry.services.ai.azure.com/api/projects/agent-framework-demos";

//creating openAI client
var chat_client = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential());

//creating adapter using MEAI
IChatClient client_adapter = chat_client.GetChatClient("gpt-4.1").AsIChatClient();

//creating Agent Framework agent
AIAgent writer = new ChatClientAgent(
    chatClient: client_adapter,
    instructions: "You are an helpful agent that writes book stories",
    name: "WriterAgent"
);
