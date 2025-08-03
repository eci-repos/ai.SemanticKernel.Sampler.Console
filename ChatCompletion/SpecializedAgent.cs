using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Runtime.CompilerServices;
using System.Text;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.ChatCompletion;

/// <summary>
/// Specialized Agent class.
/// </summary>
public class SpecializedAgent
{
   private readonly IChatCompletionService _chatService;
   private readonly AgentOptions _options;
   private readonly ChatHistory _systemChat;

   public string Specialization => _options.Specialization;

   /// <summary>
   /// Specialized Agent Constructure.
   /// </summary>
   /// <param name="chatService">chat service</param>
   /// <param name="options">options</param>
   public SpecializedAgent(IChatCompletionService chatService, IOptions<AgentOptions> options)
   {
      _chatService = chatService;
      _options = options.Value;

      // Initialize with specialized system prompt
      _systemChat = new ChatHistory();
      _systemChat.AddSystemMessage(
          $"You are a {_options.ExpertiseLevel} specializing in {_options.Specialization}. " +
          $"Your personality is {_options.Personality}. " +
          "You provide detailed, accurate information and can explain complex concepts clearly. " +
          "If asked about topics outside your specialization, you politely decline to answer " +
          "and suggest consulting a different expert.");
   }

   /// <summary>
   /// Get Response Async.
   /// </summary>
   /// <param name="query">submitted query/question</param>
   /// <param name="cancellationToken">cancelation token</param>
   /// <returns>instance of Task is returned</returns>
   public async Task<string> GetSpecializedResponseAsync(
      string query, CancellationToken cancellationToken = default)
   {
      var chat = new ChatHistory(_systemChat);
      chat.AddUserMessage(query);

      var response = await _chatService.GetChatMessageContentAsync(
          chat,
          executionSettings: new OpenAIPromptExecutionSettings
          {
             MaxTokens = 2000,
             Temperature = 0.3,  // Lower temperature for more factual responses
             TopP = 0.5
          },
          cancellationToken: cancellationToken);

      return response.Content ?? "I don't have a response for that.";
   }

   /// <summary>
   /// Stream Response Async.
   /// </summary>
   /// <param name="query">submitted query/question</param>
   /// <param name="cancellationToken">cancelation token</param>
   /// <returns>instance of Task is returned</returns>
   public async IAsyncEnumerable<string> StreamSpecializedResponseAsync(
      string query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
   {
      var chat = new ChatHistory(_systemChat);
      chat.AddUserMessage(query);

      var settings = new OpenAIPromptExecutionSettings
      {
         MaxTokens = 2000,
         Temperature = 0.3,
         TopP = 0.5
      };

      var fullResponse = new StringBuilder();
      await foreach (var chunk in _chatService.GetStreamingChatMessageContentsAsync(
         chat, settings, cancellationToken: cancellationToken))
      {
         if (chunk.Content is not null)
         {
            fullResponse.Append(chunk.Content);
            yield return chunk.Content;
         }
      }
   }

}
