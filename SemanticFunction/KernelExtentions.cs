using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using local = ai.SemanticKernel.Sampler.Console.SemanticFunction;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.SemanticFunction;

/// <summary>
/// Extended prompt template with additional functionality
/// </summary>
public static class KernelExtensions
{

   public static KernelFunction CreateFunctionFromPromptTemplate(
       this Kernel kernel,
       PromptTemplateConfig config,
       string promptTemplate)
   {
      // create execution settings (optional)
      var executionSettings = new OpenAIPromptExecutionSettings
      {
         Temperature = 0.7,
         MaxTokens = 500
      };

      // Create the function directly with prompt template config
      return KernelFunctionFactory.CreateFromPrompt(
         promptTemplate, functionName: config.Description, executionSettings: executionSettings);
   }

}
