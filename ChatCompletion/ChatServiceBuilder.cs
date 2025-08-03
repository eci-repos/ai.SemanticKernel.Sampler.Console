using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ai.SemanticKernel.Sampler.Console.ChatCompletion;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.ChatCompletion;

/// <summary>
/// [Chat Completion] Chat Service Buildeer.
/// </summary>
public class ChatServiceBuilder
{

   /// <summary>
   /// Build the Chat Service.
   /// </summary>
   /// <param name="builder">(host) builder</param>
   public static void ChatServiceBuild(HostApplicationBuilder builder)
   {
      // Configure Options (see appSettings.json)
      builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));
      builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Agent"));

      // Register Kernel as Singleton
      builder.Services.AddTransient<Kernel>(sp =>
      {
         // fetch configuration from appSettings.json
         var ollamaOptions = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
         var agentOptions = sp.GetRequiredService<IOptions<AgentOptions>>().Value;

         var builder = Kernel.CreateBuilder();

         // add standard chat service
         builder.AddOllamaChatCompletion(
                 modelId: ollamaOptions.ModelId,
                 endpoint: new Uri(ollamaOptions.Endpoint),
                 serviceId: ollamaOptions.ServiceId);

         // add specialized agent
         builder.AddOllamaChatCompletion(
             modelId: ollamaOptions.ModelId,
             endpoint: new Uri(ollamaOptions.Endpoint),
             serviceId: "specialized-agent");

         return builder.Build();
      });

      // Register Chat Completion Services
      builder.Services.AddSingleton<IChatCompletionService>(sp =>
      {
         var kernel = sp.GetRequiredService<Kernel>();
         return kernel.GetRequiredService<IChatCompletionService>();
      });

      builder.Services.AddSingleton<SpecializedAgent>(sp =>
      {
         var kernel = sp.GetRequiredService<Kernel>();
         var agentOptions = sp.GetRequiredService<IOptions<AgentOptions>>();
         var chatService = kernel.GetRequiredService<IChatCompletionService>("specialized-agent");
         return new SpecializedAgent(chatService, agentOptions);
      });

      // Register the main application service
      builder.Services.AddHostedService<ChatService>();
   }

}
