using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ai.SK.Library;
using static ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced.ActivityInfo;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced;

/// <summary>
/// Text Chuncker...
/// </summary>
public class ActivityTextChuncker
{

   /// <summary>
   /// Generates a stable <see cref="Guid"/> based on the provided activity code, section, and text.
   /// </summary>
   /// <remarks>The generated <see cref="Guid"/> is stable for the same combination of input values.
   /// This method uses a SHA-256 hash of the concatenated inputs to ensure uniqueness and 
   /// consistency.</remarks>
   /// <example>
   /// When creating ActivityChunk objects, you can use this method to assign a stable ID:
   /// new ActivityChunk {
   ///    Id = TextChuncker.StableId(activityCode, section, text),
   /// Note that this approach ensures that the same activity code, section, and text will always
   /// be mapped to the same Guid, which is useful for deduplication and consistent. If you want to
   /// do this you need to create an ID property or make sure that the Id support both get and set.
   /// </example>
   /// <param name="activityCode">A string representing the activity code. Cannot be null or empty.
   /// </param>
   /// <param name="section">A string representing the section. Cannot be null or empty.</param>
   /// <param name="text">A string representing the text. Cannot be null or empty.</param>
   /// <returns>A <see cref="Guid"/> that is deterministically derived from the input parameters.
   /// </returns>
   public static Guid StableId(string activityCode, string section, string text)
   {
      using var sha = System.Security.Cryptography.SHA256.Create();
      var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"{activityCode}|{section}|{text}"));
      var guidBytes = bytes.Take(16).ToArray();
      return new Guid(guidBytes);
   }

   /// <summary>
   /// Builds a collection of activity chunks from the provided activity corpus.
   /// </summary>
   /// <remarks>The method processes each activity in the corpus by splitting its base text into 
   /// chunks based on paragraphs. Each chunk is categorized into a section (e.g., "Overview", 
   /// "Registration", etc.) based on its content. If no specific section is identified, the chunk 
   /// is categorized as "General".  The resulting <see cref="ActivityChunk"/> objects include 
   /// metadata such as the activity code, the chunk's text, its section, a
   /// CSV-formatted tag string, and a link to the activity's section.</remarks>
   /// <param name="corpus">An array of <see cref="ActivityCorpus"/> objects representing the 
   /// source data for building chunks. Each item in the array contains the base text and 
   /// associated metadata for an activity.</param>
   /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ActivityChunk"/> objects, where each 
   /// chunk represents a categorized section of the activity text, including metadata such as 
   /// activity code, section type, and a generated link.</returns>
   public static IEnumerable<ActivityChunk> BuildChunks(ActivityCorpus[] corpus)
   {
      foreach (var item in corpus)
      {
         var chunks = TextChunker.ChunkByParagraph(item.Base).ToArray();
         foreach (var c in chunks)
         {
            var section = c.StartsWith("Overview:") ? "Overview" :
                          c.StartsWith("Registration:") ? "Registration" :
                          c.StartsWith("Participants:") ? "Participants" :
                          c.StartsWith("Location:") ? "Location" : "General";

            yield return new ActivityChunk
            {
               // Id = Guid.NewGuid(), // or use StableId(...) for consistent IDs
               ActivityCode = item.Code,
               Text = c,
               Section = section,
               TagsCsv = $"{item.Code},{section}".ToLowerInvariant(),
               Link = $"https://example.local/activities/{item.Code}#{section.ToLowerInvariant()}"
            };
         }
      }
   }

}
