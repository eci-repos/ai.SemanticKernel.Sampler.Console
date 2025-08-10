# Memory Store Embeddings


## Understanding Memory Store

A memory store in Microsoft Semantic Kernel provides a centralized, persistent location for 
storing and retrieving embeddings, enabling your AI app to remember context beyond a single 
interaction. Key benefits include durability (data survives restarts), fast semantic search 
for finding relevant information by meaning rather than exact keywords, scalability to 
handle large datasets, and flexible filtering for precise queries. By offloading embedding 
storage and search to a dedicated store, you improve performance, enable long-term contextual 
reasoning, and make your AI solutions more consistent and intelligent over time.

### Qdrant Memory Store

In Microsoft Semantic Kernel, the Qdrant memory store is a persistent vector database 
integration that replaces the default in-memory (volatile) store with scalable, searchable 
embeddings storage. Instead of losing data when your app restarts, Qdrant stores vectors 
permanently and supports high-performance similarity search using HNSW indexing with distance 
metrics like cosine, dot product, Euclidean, or Manhattan. It also allows filtering, full-text 
search, and hybrid retrieval, making it suitable for production scenarios where semantic 
memory must be both durable and queryable.

Developers integrate it in C# by adding the Microsoft.SemanticKernel.Connectors.Qdrant NuGet 
package and registering it through extension methods such as AddQdrantVectorStore() or 
WithQdrantMemoryStore(). Semantic Kernel automatically maps object properties to Qdrant 
point IDs, payloads, and vectors, with options to override mappings via attributes. The store 
works in both single-vector and multi-named vector modes, supports common data types, and can 
be configured to run against a self-hosted or cloud-hosted Qdrant instance. This makes it a 
strong choice when you need durable, production-ready semantic memory in your SK apps.

### Install Qdrant (Docker Container)
In PowerShell:

```
docker run -p 6333:6333 -p 6334:6334 -v $(pwd)/qdrant_data:/qdrant/storage qdrant/qdrant
```

Replace "$(pwd)" as needed to keep data in a different location.

