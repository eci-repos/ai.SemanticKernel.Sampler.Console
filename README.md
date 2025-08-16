# Microsoft Semantic Kernel Sampler

**Semantic Kernel** is an open-source SDK by Microsoft that simplifies integrating **AI models** (like LLMs) into applications, enabling developers to combine traditional programming with AI-powered semantic functions.

## Goals  

1. **Bridge AI and Traditional Code** - Seamlessly integrate LLMs (OpenAI GPT, Azure AI, Hugging Face) with conventional programming.  

2. **Orchestrate AI and Native Functions** - Combine AI-driven semantic functions with native code for structured workflows.  

3. **Simplify AI-Powered Development** - Reduce boilerplate code for prompts, memory, and AI orchestration.  

4. **Support Multi-Model & Multi-Service** - Work with multiple AI providers (OpenAI, Azure, local models) and APIs.  

5. **Enable Pluggable Architecture** - Modular design for memory, connectors, and plugins.  

## Key Components  

1. **Kernel (Core Orchestrator)** - Coordinates **native functions** (code) and **semantic functions** (AI prompts) and Manages execution plans, context, and memory.  

2. **Semantic Functions** - AI-powered functions using **natural language prompts** (e.g., GPT-4 summarization).  

3. **Native Functions** - Traditional code (C#, Python, Java) that integrates with AI, for xample: Fetching DB data before processing with an LLM.  

4. **Plugins (Skills)** - Reusable modules for tasks (e.g., `EmailPlugin` for sending emails).  

5. **Connectors** - Pre-built integrations (OpenAI, Azure AI, databases, APIs).  

6. **Memory & Embeddings** - Stores/retrieves contextual data (vector DBs like Pinecone), and enables **semantic search** over documents.  

7. **Planners** - Breaks complex tasks into subtasks using AI (e.g., "Plan a vacation" → book flights, hotels).  

8. **Templates & Prompt Engineering** - Tools for optimizing AI prompts (supports **Handlebars**).  

## Use Cases  
- **AI Assistants** (chatbots with memory)  
- **Automated Workflows** (document processing)  
- **Semantic Search & RAG**  
- **AI-Augmented Apps** (e.g., coding assistants)  

**Supported Languages:** C#, Python, Java.  

## Code Sampler

This project have code samples for:

- **Chat Completion** - Build a Chat Service
- **Auto Function Calling** - Build a Travel Planner
- **Semantic Function** - Build Prompt Template and apply it
- **Prompt Template Loader** - Prompt Loading
- **Web Research Agent** - Research Single Topic
- **Web Research Agent** - Research Multi-Topics and Compare those
- **Memory Store & Embeddings** - Activity Support
- **Handlebars Templating** - Show Handlebars basic and complex code samples

The samples Code was written using preview and Semantic Kernel libraries that are under development 
and therefore those will need to be reviewed and updated in a timely fashion.

## References and Links

https://learn.microsoft.com/en-us/semantic-kernel/overview/