using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using OpenAI.VectorStores;
using Qdrant.Client;
using System.Text;

using static ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced.ActivityInfo;
using static Azure.Core.HttpHeader;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced;

public class ContextSearchManager
{
   private ContextManagerConfig _config;
   private Kernel _kernel;

   private QdrantVectorStore _vectorStore;
   private QdrantCollection<Guid, ActivityChunk> _collection;

   /// <summary>
   /// Initializes a new instance of the <see cref="ContextSearchManager"/> class,  configuring it
   /// with the specified settings for managing context-based searches.
   /// </summary>
   /// <remarks>This constructor sets up the necessary components for context-based searches, 
   /// including a
   /// connection to a Qdrant vector store and integration with Ollama for  chat completions and 
   /// embedding generation.
   /// The Qdrant client defaults to  localhost:6333 unless otherwise specified in the 
   /// <paramref name="config"/>  The following components are configured: <list type="bullet"> 
   /// <item><description>Qdrant vector store for storing and retrieving embeddings.
   /// </description></item> <item><description>Ollama chat completion for generating
   /// conversational responses.</description></item> <item><description>Ollama embedding 
   /// generator for creating vector embeddings.</description></item> </list></remarks>
   /// <param name="config">The configuration settings used to initialize the context search 
   /// manager,  including model endpoints, embedding models, and vector store connection details.
   /// </param>
   public ContextSearchManager(ContextManagerConfig config)
   {
      _config = config;

      // Qdrant connection (defaults to localhost:6333; add API key/URI if needed)
      var qdrantClient = new QdrantClient(host: config.StoreHost, port: config.StorePort);

      // Build kernel with Ollama chat + embeddings
      IKernelBuilder builder = Kernel.CreateBuilder();

      // Chat completion via Ollama (LLM = Llama 3)
      builder.AddOllamaChatCompletion(
          modelId: config.Model,
          endpoint: new Uri(config.ModelEndpoint));

      // Embeddings via Ollama
      builder.AddOllamaEmbeddingGenerator(
         modelId: config.EmbeddingModel,
         endpoint: new Uri(config.ModelEndpoint));

      // Register Qdrant Vector Store
      builder.Services.AddSingleton(qdrantClient);
      builder.Services.AddQdrantVectorStore(); // DI extension from SK Qdrant connector

      _kernel = builder.Build();
   }

   /// <summary>
   /// Gets the service responsible for generating embeddings from string inputs.
   /// </summary>
   /// <remarks>This property retrieves the embedding generation service from the dependency 
   /// injection container. Ensure that the required service is registered in the container before
   /// accessing this property.</remarks>
   private IEmbeddingGenerator<string, Embedding<float>> embeddingService
      => _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

   /// <summary>
   /// Creates and returns a configured instance of <see cref="QdrantVectorStore"/> for use with 
   /// the specified <see cref="Kernel"/>.
   /// </summary>
   /// <remarks>The returned <see cref="QdrantVectorStore"/> is initialized with a client connected 
   /// to the host specified in the configuration. The embedding generator is resolved from the
   /// provided <paramref name="kernel"/>
   /// to enable automatic embedding functionality.</remarks>
   /// <param name="kernel">The <see cref="Kernel"/> instance used to resolve required services for 
   /// the vector store.</param>
   /// <returns>A new instance of <see cref="QdrantVectorStore"/> configured with the necessary 
   /// embedding generator and client options.</returns>
   public QdrantVectorStore PrepareVectorStore()
   {
      // Configure Qdrant Vector Store and let it auto-embed via the generator
      _vectorStore = new QdrantVectorStore(
          qdrantClient: new QdrantClient(_config.StoreHost),
          ownsClient: false,
          options: new QdrantVectorStoreOptions { EmbeddingGenerator = embeddingService }
      );

      return _vectorStore;
   }

   /// <summary>
   /// -- MAKE SURE THAT THE Qdrant SERVER IS RUNNING --
   /// Prepares a collection by creating or retrieving a strongly typed collection, ensuring its 
   /// existence, and upserting activity chunks with automatically computed embeddings.
   /// </summary>
   /// <remarks>This method retrieves a collection from the configured vector store, ensures the 
   /// collection exists,  and processes the provided corpus into chunks. The chunks are then 
   /// upserted into the collection, with embeddings automatically generated during the process.
   /// </remarks>
   /// <param name="corpus">An array of <see cref="ActivityCorpus"/> objects representing the source
   /// data to be processed into chunks and upserted into the collection.</param>
   public async void PrepareCollection(ActivityCorpus[] corpus)
   {
      // Create/get a strongly typed collection
      var vectorStore = _kernel.Services.GetRequiredService<QdrantVectorStore>();
      _collection = vectorStore.GetCollection<Guid, ActivityChunk>(_config.DefaultCollection);

      // Ingest -upsert chunks (embeddings computed automatically)
      await _collection.EnsureCollectionDeletedAsync(); // start fresh
      await _collection.EnsureCollectionExistsAsync();

      // Prepare chunks
      var embeddingGenerator = 
         _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
      var chunks = TextChuncker.BuildChunks(corpus).ToList();

      var texts = chunks.Select(c => c.Text).ToList();

      // Generate embeddings
      var gen = await embeddingGenerator.GenerateAsync(
          chunks.Select(c => c.Text).Where(t => !string.IsNullOrWhiteSpace(t)),
          new EmbeddingGenerationOptions { ModelId = _config.EmbeddingModel }
      );
      for (int i = 0; i < chunks.Count; i++)
      {
         chunks[i].Embedding = gen[i].Vector; // <— ReadOnlyMemory<float>
      }

      await _collection.UpsertAsync(chunks);

      System.Console.WriteLine($"Upserted {chunks.Count} chunks.");
   }

   /// <summary>
   /// Prepares the text search functionality by wrapping the vector store collection as a text 
   /// search provider and exposing it as a plugin for model interaction.
   /// </summary>
   /// <remarks>This method configures a text search provider using the vector store collection and 
   /// embedding service. It also creates and registers a plugin that allows the model to perform 
   /// text searches, returning normalized search results with name, value, and link information.
   /// </remarks>
   public void PrepareTextSearch()
   {
      // Wrap the vector store collection as a TextSearch provider
#pragma warning disable SKEXP0001
      var textSearch = new VectorStoreTextSearch<ActivityChunk>(_collection, embeddingService);

      // Optional: expose as a plugin the model can call (returns normalized TextSearchResult with
      // Name/Value/Link)
      var searchPlugin = textSearch.CreateWithGetTextSearchResults(_config.TextSearchPluginName);
      _kernel.Plugins.Add(searchPlugin);
   }

   /// <summary>
   /// Asynchronously generates a response to the specified user question using a prompt template.
   /// </summary>
   /// <remarks>This method uses the specified prompt template factory to format the prompt and 
   /// invokes the kernel to generate a response. The response is based on the provided user 
   /// question and the default prompt configuration.</remarks>
   /// <param name="userQuestion">The question or query provided by the user. Cannot be null or 
   /// empty.</param>
   /// <param name="templateFactory">A factory for creating prompt templates in the Handlebars 
   /// format. This determines how the prompt is structured.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the 
   /// generated response as a string.</returns>
   public async Task<string> GetPromptResponse(
      string userQuestion,
      HandlebarsPromptTemplateFactory templateFactory)
   {
      var args = new KernelArguments { ["query"] = userQuestion };
      var answer = await _kernel.InvokePromptAsync(
          _config.DefaultPrompt,
          args,
          templateFormat: HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
          promptTemplateFactory: templateFactory);
      return answer.GetValue<string>();
   }

}

