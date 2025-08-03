using Microsoft.SemanticKernel;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.SemanticFunction;

public class PromptTemplateConfig
{

   /// <summary>
   /// Input Parameter
   /// </summary>
   /// <remarks>
   /// Note that Microsof.SemanticKernel.PromptTemplateConfig has a definition similar to this
   /// class
   /// </remarks>
   public class InputParameter
   {
      public string Name { get; set; } = string.Empty;
      public string Description { get; set; } = string.Empty;
      public string DefaultValue { get; set; } = string.Empty;
      public bool IsRequired { get; set; } = false;
   }

   /// <summary>
   /// Configuration for the semantic function
   /// </summary>
   public class Config
   {
      public string Description { get; set; } = string.Empty;
      public InputParameter[] InputParameters { get; set; } = Array.Empty<InputParameter>();
   }

}
