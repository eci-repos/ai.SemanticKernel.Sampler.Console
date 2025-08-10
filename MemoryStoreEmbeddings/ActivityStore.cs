using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.MemoryStoreEmbeddings;

public class ActivityStore
{

   public static async void ActivityStoreSample(Kernel kernel, QdrantVectorStore store)
   {
      // Create/ingest/search
      var notes = store.GetCollection<ulong, ActivityNote>("activity-notes");
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
      await foreach (var hit in notes.SearchAsync("active full trail kike activities", top: 3))
      {
         System.Console.WriteLine($"{hit.Score:0.000}  {hit.Record.Text}");
      }

      // Use retrieved snippets to ground your chat reply (unchanged from before)
      var chat = kernel.GetRequiredService<IChatCompletionService>();
   }

}
