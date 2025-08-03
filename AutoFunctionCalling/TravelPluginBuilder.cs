using Microsoft.SemanticKernel;
using Microsoft.Extensions.Hosting;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.AutoFunctionCalling;

/// <summary>
/// [Auto Function Calling] Travel Plugin Builder.
/// </summary>
public class TravelPluginBuilder
{

   /// <summary>
   /// Build 
   /// </summary>
   /// <param name="builder"></param>
   public static Kernel? TravelPluginBuild(HostApplicationBuilder builder)
   {

      // Initialize the kernel
      var kernel = Kernel.CreateBuilder()
          .AddOllamaChatCompletion(modelId: "llama3", endpoint: new Uri("http://localhost:11434"))
          .Build();

      // Create custom plugins for our travel functions
      var travelPlugin = new TravelPlugin();

      kernel.ImportPluginFromObject(travelPlugin, "Travel");

      return kernel;
   }

}

