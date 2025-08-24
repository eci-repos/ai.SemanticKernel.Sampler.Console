using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced;

public class ActivityInfo
{
   public sealed class ActivityChunk
   {
      [VectorStoreKey]
      public Guid Id { get; init; } = Guid.NewGuid();

      [VectorStoreData(IsFullTextIndexed = true)]
      public required string ActivityCode { get; init; }

      [VectorStoreData(IsFullTextIndexed = true)]
      public required string Text { get; init; }

      [VectorStoreData]
      public required string Section { get; init; }

      [VectorStoreData]
      public required string TagsCsv { get; init; }

      [VectorStoreData]
      public required string Link { get; init; }

      // IMPORTANT: non-nullable vector; you must populate it before UpsertAsync.
      [VectorStoreVector(1024)]
      public ReadOnlyMemory<float> Embedding { get; set; }
   }
}
