using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced;

public class ContextManagerConfig
{

   public string ModelEndpoint { get; set; } = "http://localhost:11434";
   public string Model { get; set; } = "llama3";
   public string EmbeddingModel { get; set; } = "mxbai-embed-large";

   // Qdrant default port is 6333, but we use 6334 for gRPC communication
   public string StoreEndpoint { get; set; } = "http://localhost:6334";
   public int StorePort { get; set; } = 6334;

   public int VectorSize { get; set; } = 1024; // embedding size
   public string DefaultCollection { get; set; } = "knowledge-base";
   public string ApiKey { get; set; } = "ollama";
   public string StoreHost { get; set; } = "localhost";
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
