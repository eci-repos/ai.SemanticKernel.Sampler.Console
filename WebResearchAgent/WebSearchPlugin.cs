using Microsoft.SemanticKernel;
using HtmlAgilityPack;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.ResearchAgent;

/// <summary>
/// Custom Plugin for Web Search using duckduckgo search.  
/// </summary>
/// <usage>kernel.Plugins.AddFromType<WebSearchPlugin>();</usage>
public class WebSearchPlugin
{

   [KernelFunction]
   public async Task<string> SearchAsync(string query, int count = 5)
   {
      // Use HttpClient to scrape DuckDuckGo or other search engines
      using var client = new HttpClient();
      client.Timeout = TimeSpan.FromSeconds(500);  // 500 seconds

      var response = await client.GetStringAsync(
         $"https://html.duckduckgo.com/html/?q={Uri.EscapeDataString(query)}");

      // Parse HTML response (using HtmlAgilityPack or AngleSharp)
      var doc = new HtmlDocument();
      doc.LoadHtml(response);

      // Extract results and return as formatted text
      var results = doc.DocumentNode.SelectNodes("//div[contains(@class, 'result')]")
          .Take(count)
          .Select(node => new {
             Title = node.SelectSingleNode(".//h2/a")?.InnerText,
             Url = node.SelectSingleNode(".//h2/a")?.GetAttributeValue("href", ""),
             Snippet = node.SelectSingleNode(".//div[contains(@class, 'snippet')]")?.InnerText
          });

      return string.Join("\n\n", 
         results.Select(r => $"Title: {r.Title}\nURL: {r.Url}\nSnippet: {r.Snippet}"));
   }

}
