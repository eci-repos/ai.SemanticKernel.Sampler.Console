using ai.SemanticKernel.Sampler.Console.SemanticFunction;
using HandlebarsDotNet;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.HandlebarsTemplates;


public class HandlebarsTemplating
{

   /// <summary>
   /// Basic Template showcasing Hendlebars feature.
   /// </summary>
   /// <returns></returns>
   public static async Task<string> BasicTemplate()
   {
      // Initialize the kernel// 1. Initialize Kernel
      var builder = Kernel.CreateBuilder();
      builder.AddOllamaChatCompletion(
          modelId: "llama3",
          endpoint: new Uri("http://localhost:11434")
      );
      var kernel = builder.Build();

      // Define Handlebars Template
      var handlebarsTemplate = """
         {{! Basic variable injection }}
         Hello {{name}}! 
    
         {{! Loop with index }}
         Your tasks for {{date}}:
         {{#each tasks}}
         {{add @index 1}}. {{this}}
         {{/each}}
    
         {{! Conditional }}
         {{#if (equal priority "high")}}
         Priority alert!
         {{/if}}
       """;

      // Prepare template config
      var handlebarsTemp = new PromptTemplateConfig()
      {
         Template = handlebarsTemplate,
         TemplateFormat = "handlebars",
         Name = "ExplainTopics"
      };

      // Configure Handlebars + register custom helpers
      var hbsOptions = new HandlebarsPromptTemplateOptions
      {
         RegisterCustomHelpers = (registerHelper, _, __) =>
         {
            // alias: equal(left, right) -> bool
            registerHelper("equal", (ctx, args) =>
                object.Equals(args[0], args[1]));
         }
      };
      var templateFactory = new HandlebarsPromptTemplateFactory(hbsOptions);

      // Create function 
      var function = kernel.CreateFunctionFromPrompt(
         handlebarsTemp, templateFactory
      );

      // Invoke with arguments
      var result = await kernel.InvokeAsync(
          function,
          new KernelArguments
          {
             { "name", "Coco" },
             { "date", DateTime.Now.ToString("D") },
             { "tasks", new List<string> { "Code review", "Write docs", "Test SK integration" } },
             { "priority", "high" }
          }
      );

      var outText = result.ToString();
      System.Console.WriteLine(outText);

      return outText;
   }

   /// <summary>
   /// User Plugin to get user info based on given ID.
   /// </summary>
   public sealed class UserPlugin
   {
      [KernelFunction]
      public Task<object> GetUserProfile(string userId)
          => Task.FromResult<object>(
             new { name = "Ava", skillLevel = 7, interests = new[] { "technology", "design" } });
   }

   /// <summary>
   /// Basic Template showcasing Hendlebars feature.
   /// </summary>
   /// <returns></returns>
   public static async Task<string> ComplexTemplate()
   {
      // Initialize the kernel// 1. Initialize Kernel
      var builder = Kernel.CreateBuilder();
      builder.AddOllamaChatCompletion(
          modelId: "llama3",
          endpoint: new Uri("http://localhost:11434")
      );
      var kernel = builder.Build();
      kernel.Plugins.AddFromType<UserPlugin>("User");

      var template = """
         {{#*inline "commonFooter"}}
         You're receiving this because you subscribed to learning updates.
         Unsubscribe: {{unsubscribeUrl}}
         {{/inline}}
       
         {{set "user" (User-GetUserProfile userId=userId)}}
    
         Subject: Weekly Learning Report for {{user.name}}
    
         {{#if (gt user.skillLevel 5)}}
         Here are advanced topics we recommend:
         {{else}}
         Here are beginner-friendly topics to start with:
         {{/if}}
    
         {{#each recommendations}}
         - {{this.topic}} ({{this.difficulty}})
         {{/each}}
    
         {{#if (includes user.interests "technology")}}
         Since you're interested in tech, check out our latest AI articles!
         {{/if}}
    
         {{> commonFooter}}
       """;

      // Configure Handlebars + register custom helpers
      var hbsOptions = new HandlebarsPromptTemplateOptions
      {
         RegisterCustomHelpers = (registerHelper, _, __) =>
         {
            registerHelper("gt", (ctx, args) =>
            {
               var a = Convert.ToDouble(args[0]);
               var b = Convert.ToDouble(args[1]);
               return a > b;
            });

            registerHelper("includes", (Context _, Arguments a) => {
               var haystack = a[0]; var needle = a[1];
               if (haystack is string s) return s.Contains($"{needle}");
               if (haystack is System.Collections.IEnumerable e)
                  foreach (var x in e) if (Equals(x, needle)) return true;
               return false;
            });
         }
      };

      // Create function with proper configuration
      var function = kernel.CreateFunctionFromPrompt(
          new()
          {
             Template = template,
             TemplateFormat = "handlebars"
          },
          new HandlebarsPromptTemplateFactory(hbsOptions)
      );

      var result = await kernel.InvokeAsync(function, new KernelArguments {
         { "userid", "12345" },
         { "unsubscribeUrl", "https://example.com/unsub" },
         { "recommendations", new[] {
            new { topic = "Neural Networks", difficulty = "Intermediate" },
            new { topic = "Python Basics", difficulty = "Beginner" }
         }}
       });

      var outText = result.ToString();
      System.Console.WriteLine(outText);

      return outText;
   }

}
