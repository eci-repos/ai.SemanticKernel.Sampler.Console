using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.SemanticFunction;

public class SemanticFunction
{

   public static async Task RunAsync()
   {
      var kernel = Kernel.CreateBuilder()
          .AddOllamaChatCompletion(
              modelId: "llama3",
              endpoint: new Uri("http://localhost:11434"))
          .Build();

      // define the Prompt Template configuration
      var promptConfig = new PromptTemplateConfig
      {
         Description = "Generate a creative product description",
         InputVariables = new List<InputVariable>
         {
            new InputVariable
            {
               Name = "product",
               Description = "The name of the product to describe",
               IsRequired = true
            },
            new InputVariable
            {
               Name = "tone",
               Description = "The tone of voice (e.g., professional, casual, funny)",
               Default = "professional"
            },
            new InputVariable
            {
               Name = "length",
               Description = "Length of the description",
               Default = "medium"
            }
         }
      };

      // define our Prompt Template
      string promptTemplate = """
         Generate a {{$length}} product description for {{$product}}.
         Tone: {{$tone}}.

         Product description:
         """;

      // Create our extended template
      var function = kernel.CreateFunctionFromPrompt(
          promptConfig, promptTemplate);

      // Create arguments
      var arguments = new KernelArguments
      {
         { "product", "self-heating coffee mug" },
         { "tone", "enthusiastic" },
         { "length", "detailed" }
      };

      var result = await function.InvokeAsync(kernel, arguments);
      System.Console.WriteLine(result.GetValue<string>());
   }

}

