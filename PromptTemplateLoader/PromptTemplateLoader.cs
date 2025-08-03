using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.PromptTemplateLoader;

public class PromptTemplateLoader
{

   public static async Task TemplateLoader()
   {
      // Create kernel with template factory
      var builder = Kernel.CreateBuilder();
      builder.AddOllamaChatCompletion(
         modelId: "llama3", endpoint: new Uri("http://localhost:11434"));
      builder.Plugins.AddFromPromptDirectory("Prompts");
      var kernel = builder.Build();

      // Verify plugin loading
      if (!kernel.Plugins.TryGetFunction(
         "Prompts", "ProductDescription", out var function))
      {
         throw new FileNotFoundException(
            "Prompt not found. Check if 'Prompts/ProductDescription/' exists in output directory.");
      }

      // prepare input variables
      var featuresList = new List<string>
         { "Low power consumption", "Thin", "Supports AI" };
      var featuresText = string.Join("\n- ", featuresList.Prepend(""));

      // Invoke a loaded prompt
      var result = await kernel.InvokeAsync(
          function, // Subdirectory name
          new KernelArguments() // Optional default arguments
          {
             {"product", "smart notebook"},
             {"tone", "professional"},
             {"features", featuresText }
          });

      System.Console.WriteLine(result);
   }

}
