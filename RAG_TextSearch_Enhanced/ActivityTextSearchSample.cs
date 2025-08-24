using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced;

public class ActivityTextSearchSample
{

   public static async Task RunAsync()
   {
      ContextManagerConfig config = new ContextManagerConfig();

      ContextSearchManager manager = new ContextSearchManager(config);
      manager.PrepareVectorStore();
      manager.PrepareCollection(ActivityCorpus.GetSampleCorpus());
      manager.PrepareTextSearch();

      var templateFactory = new HandlebarsPromptTemplateFactory();

      foreach (var q in config.UserQuestions)
      {
         System.Console.WriteLine("\n====================================================");
         System.Console.WriteLine($"Q: {q}\n");

         var answer = await manager.GetPromptResponse(q, templateFactory);

         System.Console.WriteLine(answer);
      }
   }

}

