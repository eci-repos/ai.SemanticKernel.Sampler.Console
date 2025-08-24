using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced.ActivityInfo;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced;

/// <summary>
/// Text Chuncker...
/// </summary>
public class TextChuncker
{

   /// <summary>
   /// Smart Chunking by adding overlap to preserve context accross boundaries by targeting chuncks
   /// by token count, not character.
   /// </summary>
   /// <remarks>
   /// Where you build the corpus, swap ChunkByParagraph(...) with TokenAwareChunks(...). Later 
   /// you can plug in a real token counter (e.g., tiktoken/SharpToken) for exactness.
   /// </remarks>
   /// <param name="text">The input text to be split into chunks. Paragraphs are separated by two 
   /// consecutive newline characters.</param>
   /// <param name="maxTokens">number of tokens (default: 320)</param>
   /// <param name="overlapTokens">overlap tokens (default: 40)</param>
   /// <param name="countTokens">provide your tokens counter</param>
   /// <returns>text chuck is returned</returns>
   public static IEnumerable<string> TokenAwareChunks(
      string text, int maxTokens = 320, int overlapTokens = 40, 
      Func<string, int> countTokens = null!)
   {
      countTokens ??= s => 
         Math.Max(1, s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length); // stub
      var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      int i = 0;

      while (i < words.Length)
      {
         int take = 0, tokens = 0;
         while (i + take < words.Length && tokens + countTokens(words[i + take]) <= maxTokens)
         {
            tokens += countTokens(words[i + take]);
            take++;
         }
         yield return string.Join(' ', words.Skip(i).Take(take));

         // step forward but keep an overlap
         int back = 0; int kept = 0;
         while (kept < overlapTokens && back < take)
         { 
            kept += countTokens(words[i + take - 1 - back]); 
            back++;
         }
         i += Math.Max(1, take - back);
      }
   }

   /// <summary>
   /// Splits the input text into chunks of paragraphs, ensuring that each chunk does not exceed the 
   /// specified maximum character limit.
   /// </summary>
   /// <remarks>This method processes the input text by splitting it into paragraphs based on double
   /// newline delimiters.  It then groups paragraphs into chunks, ensuring that the total character 
   /// count of each chunk does not exceed <paramref name="maxChars"/>.  If a paragraph is too long 
   /// to fit within the limit, it is split into smaller chunks.</remarks>
   /// <param name="text">The input text to be split into chunks. Paragraphs are separated by two 
   /// consecutive newline characters.</param>
   /// <param name="maxChars">The maximum number of characters allowed in each chunk. Defaults 
   /// to 800. If a single paragraph exceeds this limit, it will be further split into smaller 
   /// chunks.</param>
   /// <returns>An enumerable collection of strings, where each string represents a chunk of text 
   /// containing one or more paragraphs.</returns>
   public static IEnumerable<string> ChunkByParagraph(string text, int maxChars = 800)
   {
      var parts = text.Split(
         "\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
      var buf = new StringBuilder();
      foreach (var p in parts)
      {
         if (buf.Length + p.Length + 2 <= maxChars)
         {
            if (buf.Length > 0) buf.AppendLine();
            buf.AppendLine(p);
         }
         else
         {
            if (buf.Length > 0) { yield return buf.ToString().Trim(); buf.Clear(); }
            if (p.Length <= maxChars) yield return p.Trim();
            else
            {
               // very long paragraph fallback
               for (int i = 0; i < p.Length; i += maxChars)
               {
                  yield return p.Substring(i, Math.Min(maxChars, p.Length - i));
               }
            }
         }
      }
      if (buf.Length > 0) yield return buf.ToString().Trim();
   }

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
         var chunks = ChunkByParagraph(item.Base).ToArray();
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
