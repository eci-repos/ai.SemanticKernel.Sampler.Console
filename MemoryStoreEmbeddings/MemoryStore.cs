using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.MemoryStoreEmbeddings;

public class MemoryStore
{

   /// <summary>
   /// Get Models (Chat Completion and Embeddings Generator)
   /// </summary>
   /// <returns></returns>
   public static Kernel GetModelKernel()
   {
      var builder = Kernel.CreateBuilder();

      // config model services...
      builder.AddOllamaChatCompletion(
         modelId: "llama3", 
         endpoint: new Uri("http://localhost:11434"));
      builder.AddOllamaEmbeddingGenerator(
         modelId: "mxbai-embed-large", 
         endpoint: new Uri("http://localhost:11434"));

      Kernel kernel = builder.Build();
      return kernel;
   }

   /// <summary>
   /// Get Vector Store (with embeddings generator)
   /// </summary>
   /// <param name="kernel"></param>
   /// <returns></returns>
   public static QdrantVectorStore GetVectorStore(Kernel kernel)
   {
      // 2) Configure Qdrant Vector Store and let it auto-embed via the generator
      var embedGen = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
      var qdrant = new QdrantVectorStore(
          qdrantClient: new QdrantClient("http://localhost:6333"),
          ownsClient: true,
          options: new QdrantVectorStoreOptions { EmbeddingGenerator = embedGen }
      );
      return qdrant;
   }

}
