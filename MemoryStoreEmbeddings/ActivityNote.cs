using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.Extensions.VectorData.ProviderServices;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.MemoryStoreEmbeddings;

public class ActivityNote
{
   [VectorStoreKeyAttribute]
   public ulong Id { get; set; }

   [VectorStoreDataAttribute(IsFullTextIndexed = true)]
   public string Text { get; set; } = string.Empty;

   // Auto-embedding: property is a string; value is derived from Text.
   [VectorStoreVectorAttribute(1024)]
   public string Embedding => this.Text;
}

