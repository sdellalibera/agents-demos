#:package Azure.Identity@1.17.1
#:package Azure.AI.OpenAI@2.1.0
#:package Microsoft.Extensions.AI.OpenAI@10.1.0-preview.1.25608.1
#:package DotNetEnv@3.1.0

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;

// Load .env — searches upward from the current directory, same as find_dotenv() in Python
DotNetEnv.Env.TraversePath().Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_FOUNDRY_ENDPOINT environment variable is not set.");

var openAIClient = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential());

// Create two specialized agents using MEAI IChatClient
IChatClient plannerAgent = openAIClient.GetChatClient("gpt-4.1").AsIChatClient();
IChatClient writerAgent = openAIClient.GetChatClient("gpt-4.1").AsIChatClient();

// Step 1: Planner agent creates a story outline
Console.WriteLine("=== Planner Agent: Creating Story Outline ===\n");
var plannerMessages = new List<ChatMessage>
{
    new(ChatRole.System, "You create concise story outlines with 3-4 bullet points."),
    new(ChatRole.User, "Create an outline for a story about an AI that learns to paint.")
};
var outline = await plannerAgent.GetResponseAsync(plannerMessages);
Console.WriteLine(outline.Message.Text);

// Step 2: Writer agent expands the outline into a full story
Console.WriteLine("\n=== Writer Agent: Writing the Story ===\n");
var writerMessages = new List<ChatMessage>
{
    new(ChatRole.System, "You write short, engaging stories (3-4 paragraphs) based on outlines."),
    new(ChatRole.User, $"Write a story based on this outline:\n{outline.Message.Text}")
};
var story = await writerAgent.GetResponseAsync(writerMessages);
Console.WriteLine(story.Message.Text);
