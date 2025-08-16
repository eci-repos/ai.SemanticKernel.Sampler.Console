# Memory Store Embeddings

The code sample demonstrates how to equip a chat assistant with **contextual memory** using 
embeddings and a **vector database**. Domain content (e.g., FAQs, past resolutions) is chunked, 
embedded into vectors, and stored in **Qdrant**. At answer time, the user’s question is embedded, 
a similarity search retrieves the most relevant snippets, and the assistant grounds its reply 
on those snippets—reducing hallucinations and keeping answers consistent and factual.

Architecturally, it uses **Microsoft Semantic Kernel** with Ollama running locally for both the 
chat model (e.g., Llama 3) and the embedding model. It follows the **modern Vector Store** path 
(instead of the legacy ```QdrantMemoryStore```): define a record with vector-store attributes 
(```[VectorStoreKey]```, ```[VectorStoreData]```, ```[VectorStoreVector(dim)]```), wire in an 
IEmbeddingGenerator, and let the Qdrant collection **auto-embed** on upsert and search. 
The runtime flow is: ensure/create the collection → upsert content → perform vector 
search (optionally with metadata filters) → build a prompt that includes retrieved 
snippets → call the chat API to generate the final answer.

Operationally, the sample calls out a few key “gotchas” and remedies: use Qdrant’s **gRPC 
port 6334** (not the REST port) to avoid HTTP/2 protocol errors; ensure the collection’s 
**vector dimension exactly matches** the embedding model (recreate the collection if you 
switch models); use the updated Vector Store methods—```EnsureCollectionExistsAsync``` / 
```EnsureCollectionDeletedAsync```—and pass **connector-specific execution settings** (e.g., 
```OllamaPromptExecutionSettings``` for temperature and max output tokens). The result is a 
compact, production-ready **RAG** loop you can extend with re-ranking, metadata filtering, 
and streaming responses as your use case grows.

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

## Understand Contextual Memory

When your app “remembers” things and uses them to answer better, that’s contextual memory. It’s 
not magic—your app saves facts (from docs or prior chats), then **retrieves only the most relevant 
bits** at answer time and feeds them to the model. That’s Retrieval-Augmented Generation (RAG).

There are three common “memory” buckets:

- Conversation (short-term): the last N messages in the chat window.
- Profile/episodic (long-term about the user): stable preferences, prior answers, tickets, etc.
- Knowledge (long-term about the domain): FAQs, manuals, wiki pages, past resolutions.

### How embeddings fit in

Embeddings turn text into numeric vectors so that “semantically similar” things end up near each 
other in vector space. You:

- Embed each chunk you want to remember (e.g., a paragraph from your FAQ).
- Store the vector + original text (and metadata) in a vector database (e.g., Qdrant).
- At question time, embed the user’s query, run a vector similarity search to find the nearest 
chunks, and
- Ground the model by inserting those chunks into the prompt so the model answers from them
rather than guessing.

In short: **embeddings are the index** that makes memory retrievable by meaning, not just keywords.

### Typical RAG loop (with Qdrant + Ollama + Semantic Kernel)

1. Ingest: chunk your sources (200–500 tokens per chunk), generate embeddings (e.g., Ollama’s 
embed model), and upsert into Qdrant with useful metadata (title, source URL, tags, timestamps).
2. Query: embed the user’s question.
3. Retrieve: vector search in Qdrant (k ≈ 4–8), optionally filter by metadata (product=“X”, 
locale=“en”, recency < 90 days).
4. (Optional) Re-rank: use a cross-encoder or scoring prompt to order the hits.
5. Ground: build a prompt section like:
1. 
```
System: Use ONLY this context to answer:
- [doc:123] "If battery drains overnight, disable Always-On Sync…"
- [kb:45]   "Long press power for 10 seconds to reset…"
```

6. Respond: call the chat model (e.g., Llama 3 via Ollama).
7. Learn: if the final answer proves useful, summarize and write back a new memory (e.g., 
“Customer X’s device model = Y; resolution = Z”) so next time you can recall it.

### Design choices that matter

- Embedding model & dimension: Pick one and stick to it per collection (Qdrant requires exact dimension match).
- Chunking: Too big → retrieval is fuzzy; too small → context is fragmented. Aim for semantically complete mini-answers.
- Metadata & filters: Tag by product/version/region/recency; combine vector search + filters for precision.
- Hybrid search: Blend vector similarity with keyword/BM25 for rare terms, codes, or part numbers.
- Re-ranking: A lightweight cross-encoder or scoring prompt often boosts answer precision.
- Prompt guards: Tell the model to prefer context, cite sources, and admit when context is insufficient.
- Memory hygiene: De-duplicate, expire stale facts, and avoid storing sensitive data unless you truly need it.

### Common pitfalls (and fixes)

- “It still hallucinates.” Add stricter system instructions and show fewer, higher-quality chunks; consider re-ranking.
- “Wrong or old info.” Add recency filters; version your collections; re-embed on major doc changes.
- “Dimension mismatch errors.” Ensure the collection’s vector size matches the embedding model; recreate the collection if you switch models.
- “Irrelevant recalls.” Improve chunking, add metadata filters, or switch to a better embedding model.

### How this feels to the end-user

They ask, “Why is my device battery draining?”
Your app silently: embeds the question → finds the 3 most relevant snippets in Qdrant → inserts them into the prompt → replies with a concise, sourced fix.
To the user, it looks like the assistant “remembered” exactly the right thing—because you retrieved it by meaning and grounded the model on it.