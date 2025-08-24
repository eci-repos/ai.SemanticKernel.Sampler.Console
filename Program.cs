using ai.SemanticKernel.Sampler.Console.AutoFunctionCalling;
using ai.SemanticKernel.Sampler.Console.ChatCompletion;
using ai.SemanticKernel.Sampler.Console.HandlebarsTemplates;
using ai.SemanticKernel.Sampler.Console.MemoryStoreEmbeddings;
using ai.SemanticKernel.Sampler.Console.PromptTemplateLoader;
using ai.SemanticKernel.Sampler.Console.ResearchAgent;
using ai.SemanticKernel.Sampler.Console.SemanticFunction;
using ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced;
using HandlebarsDotNet;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

// -------------------------------------------------------------------------------------------------
// - Note that the following is not needed for some of the examples...
var builder = Host.CreateApplicationBuilder(args);

// COMMENT OUT THE FEATURE/EXAMPLE YOU LIKE TO REVIEW - TEST
// -------------------------------------------------------------------

// [Chat Completion] - Build Chat Service
//ChatServiceBuilder.ChatServiceBuild(builder);

// [Auto Function Calling] - Build Travel Planner
//var kernel = TravelPluginBuilder.TravelPluginBuild(builder);
//await TravelPlanner.PreparePlan(kernel);

// [Semantic Function] - Build Prompt Template and apply it
//await SemanticFunction.RunAsync();

// [Prompt Template Loader] - Prompt Loading
//await PromptTemplateLoader.TemplateLoader();

// [Web Research Agent] - Single topic
//await WebResearchAgent.SingleTopicResearchAsync("climate change impacts on coastal cities");

// [Web Research Agent] - Multi Topic Comparison
//string[] relatedTopics = new[]
//    {
//        "wind turbine technology advancements 2023",
//        "solar energy efficiency improvements 2023",
//        "battery storage innovations 2023"
//    };
//await WebResearchAgent.MultiTopicComparisonAsync(relatedTopics);

// [Memory Store & Embeddings] - Activity Support
//await ActivitySupport.GetActivitySupportResponse("Are there any activities for hiking");

// [Handlebars Templating] - show handlebars basic example
//await HandlebarsTemplating.BasicTemplate();
//await HandlebarsTemplating.ComplexTemplate();

// [RAG Text Search Enhanced] - Activity Text Search Sample
await ActivityTextSearchSample.RunAsync();

// -------------------------------------------------------------------
// - Note that the following is not needed for some of the examples...
// build host and run requested service
var host = builder.Build();
await host.RunAsync();
