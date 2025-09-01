using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.AutoFunctionCalling;

public class TravelPlanner
{

   public static async Task PreparePlan(Kernel? kernel)
   {

      // Create chat service
      var chat = kernel.GetRequiredService<IChatCompletionService>();

      // Create chat history
      var history = new ChatHistory();
      history.AddUserMessage("""
         I'm planning a trip from New York to Paris on June 15th for 5 days. 
         I need to know flight options, hotel availability for 2 people, 
         the weather forecast, and how much 1000 USD is in Euros.
       """);

      // Execution settings with auto function calling enabled
      var executionSettings = new OpenAIPromptExecutionSettings
      {
         ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
         Temperature = 0
      };

      // Stream the response
      System.Console.WriteLine("Assistant: ");
      String fullResponse = "";
      await foreach (var chunk in chat.GetStreamingChatMessageContentsAsync(
          history,
          executionSettings,
          kernel))
      {
         System.Console.Write(chunk.Content ?? "");
         fullResponse += chunk.Content;
      }

      // Show the complete response at the end
      System.Console.WriteLine("\n\nComplete response:");
      System.Console.WriteLine(fullResponse);
      
   }

}
