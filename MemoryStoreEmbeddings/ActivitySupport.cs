using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using Qdrant.Client.Grpc;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.MemoryStoreEmbeddings;


/// <summary>
/// 
/// </summary>
public class ActivitySupport
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
          qdrantClient: new QdrantClient("localhost"),
          ownsClient: true,
          options: new QdrantVectorStoreOptions { EmbeddingGenerator = embedGen }
      );
      return qdrant;
   }

   /// <summary>
   /// Show how to provide "contextual memory" 
   /// </summary>
   /// <param name="kernel"></param>
   /// <param name="store"></param>
   /// <param name="userInput">user Input/Question</param>
   /// <returns></returns>
   public static async Task<string> GetPromptResponse(
      Kernel kernel, QdrantVectorStore store, string userInput)
   {
      // inquire the supported embeddings vector dimension
      //var embedGen = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
      //var dim = (await embedGen.GenerateVectorAsync("ping")).Length; // expected: 1024
      //System.Console.WriteLine(dim);

      // Create/ingest/search
      QdrantCollection<ulong, ActivityNote> notes = 
         store.GetCollection<ulong, ActivityNote>("activity-notes");

      // delete the collection if it exists
      await notes.EnsureCollectionDeletedAsync();

      await notes.EnsureCollectionExistsAsync();

      await notes.UpsertAsync(new[]
      {
         new ActivityNote
         { 
            Id = 1, 
            Text = "2025-05-01 full trail hike activity is rescheduled."
         },
         new ActivityNote
         { 
            Id = 2,
            Text = "2025-05-01 full trail hike is now cancel."
         }
      });

      // Vector search (embeddings for the query are generated automatically)
      await foreach (var hit in notes.SearchAsync("active full trail hike activities", top: 3))
      {
         System.Console.WriteLine($"{hit.Score:0.000}  {hit.Record.Text}");
      }

      // Use retrieved snippets to ground your chat reply (unchanged from before)
      var chat = kernel.GetRequiredService<IChatCompletionService>();

      // User question
      string userQuestion = "Are there any activities for hiking";

      // Vector search (example shape)
      var contextLines = new List<string>();
      await foreach (var hit in notes.SearchAsync(userQuestion, top: 4))
      {
         // Include a simple “citation” marker so users know what source was used
         contextLines.Add($"- [{hit.Record.Id}] {hit.Record.Text}");
      }
      string context = string.Join("\n", contextLines);

      // Build grounded chat history
      var history = new ChatHistory();
      history.AddSystemMessage(
          "You are a helpful activities assistant. " +
          "Use ONLY the provided Context to answer. " +
          "If the context is insufficient, say so and ask a focused follow-up.\n\n" +
          "Context:\n" + context);
      history.AddUserMessage(userQuestion);

      // Call the chat model (non-streaming)
      var settings = new OllamaPromptExecutionSettings
      {
         Temperature = 0.2f,  // creativity
         NumPredict = 300,   // max output tokens
         TopP = 0.9f
      };

      var reply = await chat.GetChatMessageContentAsync(history, settings, kernel);
      System.Console.WriteLine(reply.Content);

      return reply.Content;
   }

   public static async Task<string> GetActivitySupportResponse(string userInput)
   {
      Kernel kernel = GetModelKernel();
      QdrantVectorStore store = GetVectorStore(kernel);
      var response = await GetPromptResponse(kernel, store, userInput);
      return response;
   }

}
