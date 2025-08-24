# RAG with Text Search Provider and Store
Continue the Microsoft Semantic Kernel Saga...

## Post Follow-up: Enhanced Functionality and Production Readiness

**Retrieval Augmented Generation (RAG)** is a powerful technique that combines the strengths of retrieval-based 
models and generative models to enhance the quality and relevance of generated content. By leveraging 
external knowledge sources, RAG can produce more accurate and contextually appropriate responses.

In case you missed that previous post showcased RAG with a simple in-memory vector store, here’s an enhanced version that uses the **TextSearchProvider and TextSearchStore abstractions** to connect to a Qdrant vector database. This example includes explicit chunking of documents, correct embedding generation using the latest `IEmbeddingGenerator<string, Embedding<float>>` API, and retrieval via TextSearch that integrates directly into prompts.

Here’s a focused walkthrough of what the code delivers and why it matters.

First, it builds on your previous posts about Retrieval-Augmented Generation (RAG). Those earlier notes set the stage for “why RAG,” while this new code shows **enhanced functionality** and a more production-shaped approach: explicit **chunking**, correct **embedding** generation with the latest **`IEmbeddingGenerator<string, Embedding<float>>`** API, and seamless **retrieval via TextSearch** that plugs directly into prompts. In short, the code turns the abstract RAG recipe into a concrete, runnable pipeline that indexes domain content and reliably feeds the right snippets back to your LLM at answer time.

### Configuring and Running Test

Before you run the code, make sure you have the Qdrant vector database and Ollama LLM running locally. You can adjust the configuration in `ContextManagerConfig.cs` to match your setup, including model names, endpoints, and collection names.

## Chunking and Data Modeling

A core highlight is the **data modeling for retrieval**. The `ActivityChunk` type carries a key, normalized text, simple metadata (section, tags), a link for citations, and a **non-nullable vector field**. This enforces the invariant that all records are embedded before upsert—preventing the common “vector may not be null” failure at write time. The model also annotates fields for **full-text indexing** and **filterability**, letting you later narrow searches to, say, `Section == "Registration"` or activities with specific tags.

The ingestion flow demonstrates **chunking** and why it’s important. The sample “Activities” corpus (activities, registration, participants, locations) is split into manageable paragraphs with a lightweight, token-agnostic chunker. This improves recall (each chunk is topically tighter) and reduces prompt bloat (you retrieve just a few relevant pieces). While the example uses a simple paragraph splitter, the structure makes it clear where to drop in a token-aware strategy and overlaps when you want to maximize relevance.

## Embeddings and Vector Store

On **embeddings**, the code adopts current best practice by using **`Microsoft.Extensions.AI`**’s **`IEmbeddingGenerator`**—avoiding obsolete SK embedding interfaces. It shows two validated paths: (1) **manual embedding** before upsert (so the `Embedding` vector is always populated), or (2) **auto-generation** by configuring the Qdrant vector store with an `EmbeddingGenerator` and storing a source (e.g., `Text`) for vector creation. This dual approach both fixes the earlier compile/runtime issues and clarifies the architectural choice you have to make in production.

For the **vector store**, it uses **Qdrant** with SK’s typed **Vector Store** abstractions. You see how to **create a typed collection**, **create the collection if it doesn’t exist**, and **upsert** records that already carry embeddings. The collection annotations—`[VectorStoreKey]`, `[VectorStoreData]`, `[VectorStoreVector]`—map your class cleanly onto Qdrant’s storage and index settings, including vector dimension alignment with your embedding model.

## Text Search Integration

Retrieval is powered by **TextSearch**—specifically **`VectorStoreTextSearch<T>`**—which wraps your Qdrant collection behind an SK-friendly **TextSearchProvider/TextSearchStore** interface. The code then exposes retrieval as a **Search plugin** (via `CreateWithGetTextSearchResults`) that you can call directly inside prompts. That pluginization is the key: it keeps your prompt clean, makes retrieval **declarative**, and allows you to iterate on retrieval parameters (top-k, skip, filters) without rewriting business logic.

Prompting uses a **Handlebars** template that demonstrates the corrected **positional parameters** call for `GetTextSearchResults` (avoiding the earlier “hash parameter” runtime error). It loops through returned `results`, renders chunk **values** and **links**, and then asks the model to answer **using only** the provided sources. This pattern shows a **guardrailed RAG prompt** with inline citations, which is often the most robust way to keep answers grounded.

The code also illustrates **LLM and embedding model selection** with **Ollama** (e.g., `llama3` for chat and an embedding model suited to your chosen vector dimension). It highlights the need to **match dimensions** between your model and `[VectorStoreVector(dim)]`, and it cleanly isolates these choices so you can swap models without refactoring the pipeline.

## Operational Best Practices

Finally, the example surfaces several **operational best practices**: enforcing non-null vectors, explicitly creating collections, keeping **links** for human-auditable citations, and offering a place to add **filters** and **scoring tweaks** (e.g., constraining by section or activity code). It’s intentionally minimal where it should be (chunking, seed data) and explicit where it must be (embedding generation, TextSearch plugin, prompt wiring), so you can extend it to handle larger corpora, richer metadata, or function-calling flows.

In summary: your **previous posts cover the topic of RAG**, and **this new code shows enhanced functionality**—it operationalizes RAG in SK with up-to-date embedding generation, solid chunking, typed vector storage in Qdrant, and a clean TextSearchProvider/TextSearchStore integration that makes retrieval a first-class citizen in your prompts.

## Next Steps

Some ideas to work on next:

- While chunking work on token-aware chunking with overlaps to maximize relevance.
- Avoid re-embedding identical texts by hashing and checking for existing vectors before upsert.
- Extend embedding with batch processing for efficiency.
- Provide retrieval controls (filters + hybrid + MMR) to refine results.
- Work on sencond-stage re-ranking of retrieved results.
- Add system guardrails that forbits fabrication outside the provided sources and include structured citations.
- Work on Observability: scores, traces, and corpus gaps.

## Review MS-SQL 2025 Semantic and Vector Search 

I previously posted an Activities Atracking database that provide some ground work on chucking and embedding directly on the database side with an AI local of external model. Take a look at the database repository here:

https://github.com/eci-repos/ConvolutoDB

## Further Reading

https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.memory.semantictextmemory?view=semantic-kernel-dotnet



