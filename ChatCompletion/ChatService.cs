using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.ChatCompletion;

/// <summary>
/// Chat Service class.
/// </summary>
public class ChatService : IHostedService
{
   private readonly IChatCompletionService _chatService;
   private readonly SpecializedAgent _specializedAgent;
   private readonly ILogger<ChatService> _logger;
   private readonly IHostApplicationLifetime _appLifetime;

   /// <summary>
   /// Chat Service onstructor.
   /// </summary>
   /// <param name="chatService">chat service</param>
   /// <param name="specializedAgent">specialized agent (export)</param>
   /// <param name="logger">for logging</param>
   /// <param name="appLifetime">app live time</param>
   public ChatService(
       IChatCompletionService chatService,
       SpecializedAgent specializedAgent,
       ILogger<ChatService> logger,
       IHostApplicationLifetime appLifetime)
   {
      _chatService = chatService;
      _specializedAgent = specializedAgent;
      _logger = logger;
      _appLifetime = appLifetime;
   }
   
   /// <summary>
   /// Start service Async.
   /// </summary>
   /// <param name="cancellationToken">cancelation token</param>
   /// <returns>instance of Task is returned</returns>
   public async Task StartAsync(CancellationToken cancellationToken)
   {
      try
      {
         _logger.LogInformation("Starting specialized agent chat service with streaming...");

         System.Console.WriteLine("Chat with AI. Type 'exit' to quit.");
         System.Console.WriteLine($"Current specialization: {_specializedAgent.Specialization}");
         System.Console.WriteLine("Prefix your message with 'expert:' to talk to the specialist\n");

         var chatHistory = new ChatHistory();
         chatHistory.AddSystemMessage("You are a helpful AI assistant.");

         while (!cancellationToken.IsCancellationRequested)
         {
            System.Console.Write("You: ");
            var userInput = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput) || 
               userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
               _appLifetime.StopApplication();
               break;
            }

            if (userInput.StartsWith("expert:", StringComparison.OrdinalIgnoreCase))
            {
               var expertQuery = userInput.Substring("expert:".Length).Trim();
               await StreamExpertResponseAsync(expertQuery, cancellationToken);
            }
            else
            {
               await StreamGeneralResponseAsync(chatHistory, userInput, cancellationToken);
            }
         }
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error during chat session");
         _appLifetime.StopApplication();
      }
   }

   /// <summary>
   /// Stream Expert Response Async.
   /// </summary>
   /// <param name="query">submitted query/question</param>
   /// <param name="cancellationToken">cancelation token</param>
   /// <returns>instance of Task is returned</returns>
   private async Task StreamExpertResponseAsync(string query, CancellationToken cancellationToken)
   {
      System.Console.Write($"{_specializedAgent.Specialization} Expert: ");
      await foreach (
         var chunk in _specializedAgent.StreamSpecializedResponseAsync(query, cancellationToken))
      {
         System.Console.Write(chunk);
      }
      System.Console.WriteLine();
   }

   /// <summary>
   /// Stream General Response Async.
   /// </summary>
   /// <param name="chatHistory">chat history</param>
   /// <param name="query">submitted query/question</param>
   /// <param name="cancellationToken">cancelation token</param>
   /// <returns>instance of Task is returned</returns>
   private async Task StreamGeneralResponseAsync(
      ChatHistory chatHistory, string query, CancellationToken cancellationToken)
   {
      chatHistory.AddUserMessage(query);

      System.Console.Write("AI: ");
      var fullResponse = new StringBuilder();
      var settings = new OpenAIPromptExecutionSettings
      {
         MaxTokens = 2000,
         Temperature = 0.7
      };

      await foreach (var chunk in _chatService.GetStreamingChatMessageContentsAsync(
          chatHistory,
          settings,
          cancellationToken: cancellationToken))
      {
         if (chunk.Content is not null)
         {
            fullResponse.Append(chunk.Content);
            System.Console.Write(chunk.Content);
         }
      }
      System.Console.WriteLine();
      chatHistory.AddAssistantMessage(fullResponse.ToString());
   }

   /// <summary>
   /// Stop Chat Service Async.
   /// </summary>
   /// <param name="cancellationToken">cancelation token</param>
   /// <returns>instance of Task is returned</returns>
   public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
