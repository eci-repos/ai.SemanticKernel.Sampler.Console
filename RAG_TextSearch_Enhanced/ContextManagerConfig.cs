using ai.SK.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced;

public class ContextManagerConfig : KernelModelConfig
{

   public string TextSearchPluginName { get; set; } = "ActivitiesSearch";

   public string DefaultPrompt { get; set; } = """
      {{!-- Fetch relevant chunks from the vector store and include them as context --}}
      {{#with (ActivitiesSearch-GetTextSearchResults query 4 0)}}
      {{#each this}}
      [{{@index}}] {{Value}}
      Source: {{Link}}
      ---
      {{/each}}
      {{/with}}

      You are a helpful assistant for a Parks & Rec department. 
      Using ONLY the sources above, answer the user's question concisely.
      If you cite, use the provided Source links inline.

      User question: {{query}}
      """;

   public string[] UserQuestions { get; set; } =
   {
       "How do I register for ACT-101 and what's the fee?",
       "What are the age limits and capacity for Intro to Pickleball?",
       "Where do participants meet for the trail cleanup?",
       "What should I bring to the yoga class?"
   };

}
