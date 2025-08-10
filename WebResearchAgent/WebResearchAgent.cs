using ai.SemanticKernel.Sampler.Console.SemanticFunction;
using Google.Apis.CustomSearchAPI.v1.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

using System.ComponentModel;
using System.Text;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.ResearchAgent;

public class WebResearchAgent
{
   private readonly Kernel _kernel;

   public WebResearchAgent(string modelId = "llama3")
   {
      // create http client to communicate with Ollama services...
      HttpClient client = new HttpClient();
      client.Timeout = TimeSpan.FromMinutes(5);
      client.BaseAddress = new Uri("http://localhost:11434");

      // Initialize kernel with LLM
      var builder = Kernel.CreateBuilder();
      builder.AddOllamaChatCompletion(
         modelId: modelId,
         httpClient: client);
      _kernel = builder.Build();

      // Import plugins
      var webSearchEnginePlugin = new WebSearchPlugin();
      _kernel.ImportPluginFromObject(webSearchEnginePlugin, "WebSearch");
   }

   /// <summary>
   /// Single topic research agent.
   /// </summary>
   /// <param name="topic">topic to research</param>
   /// <param name="maxResults">maximum results (default: 3)</param>
   /// <param name="sleepMilliseconds">sleep milliseconds (default: 0)</param>
   /// <returns>a results string is returned</returns>
   public async Task<string> ResearchTopicAsync(
      string topic, int maxResults = 3, int sleepMilliseconds = 0)
   {
      string result;

      // Explicit prompt config with unambiguous constructor
      var researchFunction = _kernel.CreateFunctionFromPrompt(
          promptTemplate: """
            Research the following topic: {{$topic}}
            
            Instructions:
            1. Use the WebSearch.Search function to find up to {{$maxResults}} authoritative sources
            2. Extract key information from each source
            3. Compile a comprehensive summary with citations
            4. Highlight any controversial or disputed claims
            
            Return your findings in this format:
            ## Research Summary: {{$topic}}
            
            ### Key Findings:
            - [List key points]
            
            ### Sources:
            - [List sources with URLs]
            """,
          functionName: "ResearchFunction",
          description: "Researches a topic online"
      );

      try
      {
         var results = await _kernel.InvokeAsync(
             researchFunction,
             new() {
                { "topic", topic },
                { "maxResults", maxResults }
             }
         );
         result = results.ToString();
      }
      catch(Exception ex)
      {
         System.Console.WriteLine(ex.Message);
         result = ex.Message;
      }

      if (sleepMilliseconds > 0)
      {
         Thread.Sleep(sleepMilliseconds);
      }

      return result.ToString();
   }

   /// <summary>
   /// Multi topic comparison.
   /// </summary>
   /// <param name="topics"></param>
   /// <param name="maxResultsPerTopic"></param>
   /// <returns></returns>
   public async Task<string> CompareTopicsAsync(string[] topics, int maxResultsPerTopic = 2)
   {

      // fetch web-search results...
      string[] researchTasks = new string[topics.Length];
      for(int c = 0; c < topics.Length; c++)
      {
         researchTasks[c] = await ResearchTopicAsync(topics[c], maxResultsPerTopic);
      }

      // Pre-format the data for the prompt
      var formattedTopics = string.Join("\n", topics.Select(t => $"- {t}"));
      var formattedResults = string.Join("\n\n",
          topics.Zip(researchTasks, (topic, result) =>
              $"### {topic}\n{result}"));

      // Create comparison function with proper syntax
      var compareFunction = KernelFunctionFactory.CreateFromPrompt(
         promptTemplate: """
         Compare these topics:
         {{$formattedTopics}}
    
         Research Results:
         {{$formattedResults}}
    
         Provide a detailed comparison highlighting:
         - Key similarities
         - Important differences
         - Potential relationships
         - Areas needing further research
         """,
         functionName: "CompareFunction",
         description: "Compares research results"
      );

      var args = new KernelArguments();
      args.Add("formattedTopics", formattedTopics);
      args.Add("formattedResults", formattedResults);

      return (await _kernel.InvokeAsync(compareFunction, args)).ToString();
   }

   /// <summary>
   /// Invoke the single topic research agent async.
   /// </summary>
   /// <param name="topic">topic to research</param>
   /// <param name="maxResults">maximum results (default: 3)</param>
   /// <param name="outToConsole">true to output to console (default: true)</param>
   /// <returns>a results string is returned</returns>
   public static async Task<string> SingleTopicResearchAsync(
      string topic, int maxResults = 3, bool outToConsole = true)
   {
      var agent = new WebResearchAgent();
      var result = await agent.ResearchTopicAsync(topic, maxResults);
      if (outToConsole)
      {
         System.Console.WriteLine(result);
      }
      return result.ToString();
   }

   /// <summary>
   /// Invoke the multi topic comparison agent async.
   /// </summary>
   /// <param name="topics">related topics</param>
   /// <param name="maxResultsPerTopic">maximum results per topic</param>
   /// <param name="outToConsole">true to output to console (default: true)</param>
   /// <returns>a results string is returned</returns>
   public static async Task<string> MultiTopicComparisonAsync(
      string[] topics, int maxResultsPerTopic = 2, bool outToConsole = true)
   {
      var agent = new WebResearchAgent();
      var result = await agent.CompareTopicsAsync(topics, maxResultsPerTopic);
      if (outToConsole)
      {
         System.Console.WriteLine(result);
      }
      return result.ToString();
   }

}

