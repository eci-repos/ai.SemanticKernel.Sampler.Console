using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ai.SemanticKernel.Sampler.Console.ChatCompletion;
using ai.SemanticKernel.Sampler.Console.AutoFunctionCalling;
using ai.SemanticKernel.Sampler.Console.SemanticFunction;
using ai.SemanticKernel.Sampler.Console.PromptTemplateLoader;

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
await PromptTemplateLoader.TemplateLoader();

// -------------------------------------------------------------------
// - Note that the following is not needed for some of the examples...
// build host and run requested service
var host = builder.Build();
await host.RunAsync();
