# Microsoft Semantic Kernel Sampler

The **Microsoft Semantic Kernel (SK)** represents a much-needed SDK that finally bridges the gap between AI models and enterprise applications. From my standpoint, it is a welcome advancement because it directly addresses the complexity of weaving natural language intelligence into real business workflows. Rather than forcing developers to manage ad-hoc prompts, scattered integrations, or heavy orchestration code, SK offers a unified and modular framework—spanning chat services, auto function calling, semantic functions, prompt management, embeddings with memory, web research agents, and even templating. This breadth of capability ensures that AI can be consistently embedded into enterprise systems with scalability, maintainability, and reliability.

What excites me most is how SK’s design anticipates enterprise realities: domain specialization, prompt versioning, function auto-invocation, vectorized memory with Qdrant, and resilient error handling. These features aren’t just technical add-ons—they reflect a deeper understanding of what it takes to deliver production-grade AI assistants that adapt dynamically to user intent and organizational knowledge. For me, SK is not just another SDK; it is a strategic enabler of AI-driven applications, empowering developers to transform conversational interfaces into intelligent agents that operate seamlessly alongside existing business systems.

**SK** is a **multi-language SDK** available in **C# (.NET)**, **Python**, and **Java**, with active community contributions expanding its reach. Because it runs on top of widely supported runtimes like **.NET, Python, and the JVM**, SK is inherently **cross-platform**—working seamlessly on **Windows, Linux, and macOS**.  

This flexibility allows teams to adopt SK within their preferred development stacks and operating environments, whether for **enterprise-grade backends in .NET**, **data workflows in Python**, or **large-scale services in Java**, while maintaining a consistent and unified programming model for integrating AI into applications.

This project showcase SK with (C#) code samples for:

- **Chat Completion** - Build a Chat Service
- **Auto Function Calling** - Build a Travel Planner
- **Semantic Function** - Build Prompt Template and apply it
- **Prompt Template Loader** - Prompt Loading
- **Web Research Agent** - Research Single Topic
- **Web Research Agent** - Research Multi-Topics and Compare those
- **Memory Store & Embeddings** - Activity Support
- **Handlebars Templating** - Show Handlebars basic and complex code samples

The samples are written using preview and Semantic Kernel libraries that are under development 
and therefore those will need to be reviewed and updated in a timely fashion, I plan to reveiw 
those periodically to the code up to date. Find the code samples in:

```
https://github.com/eci-repos/ai.SemanticKernel.Sampler.Console
```

Details of each sample are provided below.

### Chat Completion - Build a Chat Service

This Semantic Kernel chat completion implementation demonstrates an AI assistant solution that combines Microsoft's Semantic Kernel with Ollama's local LLM capabilities. The architecture features a dual-mode chat system—supporting both general conversations and domain-specialized interactions—with real-time streaming responses for optimal user experience. Leveraging dependency injection, configuration management, and modular design, the solution offers enterprise-grade features including:

Specialized agent expertise with customizable knowledge domains
Token-by-token streaming for natural conversation flow
Advanced Semantic Kernel integrations (planners, prompt templates, memory, and native functions)
Configuration validation and error handling for reliability
Extensible architecture ready for multi-agent routing and dynamic specialization
The implementation serves as a foundation for developing sophisticated, domain-aware AI assistants while maintaining clean code practices and scalability for enterprise deployment.

### Auto Function Calling - Build a Travel Planner

Semantic Kernel's Auto Function Calling is an advanced feature that enables AI models to dynamically 
invoke predefined native functions (or plugins) during conversation or task execution based on 
contextual understanding of user requests, eliminating the need for explicit programming of each 
function call. This capability leverages the AI's natural language processing to automatically 
select appropriate functions, extract required parameters from the conversation, execute them in 
logical sequences, and seamlessly integrate results into responses, effectively transforming 
the AI into an intelligent agent that can interact with external systems and services. The system 
handles complex scenarios like parameter type conversion, error recovery, and multi-step operations 
while maintaining conversational flow, with features like automatic function selection (choosing 
relevant tools from available plugins), parameter extraction (parsing values from natural language), 
and adaptive execution (chaining calls when needed), all while supporting streaming responses for 
real-time interaction. Developers implement this by decorating functions with metadata 
([KernelFunction] and [Description] attributes) and configuring the kernel with 
ToolCallBehavior.AutoInvokeKernelFunctions, allowing the AI to autonomously decide when to use 
functions like weather checks, calculations, or database queries based on the user's intent, 
significantly reducing manual orchestration code while enabling sophisticated AI-agent behaviors 
in applications like travel planning, customer support, or data analysis workflows.

### Semantic Function - Build Prompt Template and apply it

Semantic Kernel's Semantic Functions provide a powerful abstraction for integrating AI-powered
natural language processing into applications by encapsulating prompt templates, configuration,
and execution logic into reusable components. These functions enable developers to define
dynamic prompts with variables (like {{$input}}), configure parameters (default values,
descriptions, and constraints), and integrate with multiple AI services
(OpenAI, Azure OpenAI, etc.) while maintaining a consistent programming interface. Key features
include template-based prompt engineering with conditional logic, structured output handling,
parameter validation, and seamless orchestration within workflows—allowing developers to build
complex AI pipelines while abstracting LLM-specific details. Semantic Functions support
context-aware chaining (where outputs flow between functions), prompt versioning, and
model-agnostic execution, making them ideal for scenarios like content generation,
data extraction, or decision-making systems that require maintainable, scalable AI integration
with minimal boilerplate code. The framework also handles retries, token management, and
temperature control while providing hooks for custom logging and monitoring.

### Prompt Template Loader - Prompt Loading

Semantic Kernel's **`PromptTemplateFactory.Load`** method addresses the critical
need for **structured, maintainable prompt management** in AI applications by 
providing a standardized way to **load and organize prompt templates** from 
the filesystem. This feature enables **modular prompt engineering** by 
automatically discovering and parsing template files (`skprompt.txt`) with 
their associated configurations (`config.json`) from designated directories, 
supporting **version control** and **team collaboration**. Key capabilities include:  

- **Template Discovery**: Scans directories to load prompts dynamically, 
reducing hardcoded strings in application code.  
- **Configuration Integration**: Associates each template with metadata 
(name, description) and input parameters (defaults, validation rules) defined in JSON.  
- **Multi-Format Support**: Handles both **Handlebars** (loops/conditionals) 
and **Semantic Kernel** basic syntax (`{{$variable}}`).  
- **Variable Prepopulation**: Accepts default `KernelArguments` during loading 
for consistent template rendering.  
- **Caching**: Improves performance by caching parsed templates.  

By decoupling prompts from application logic, this system enables 
**prompt versioning**, **environment-specific configurations** (e.g., dev vs. prod), 
and **reuse across projects**, making it ideal for scaling LLM-powered features while 
maintaining auditability and consistency. The factory pattern ensures prompts remain 
**editable without recompilation**, streamlining iterative improvements during AI development.

### Web Research Agent - Research Single Topic, Multi-Topics and Compare those

This code implements a Web Research Agent using Microsoft's Semantic Kernel and Ollama's local 
LLM capabilities. The agent performs online research on given topics by leveraging web search 
functionality and summarizing findings through an LLM. Key features include configurable timeout 
settings for HTTP requests, robust error handling, and support for both single-topic research 
and multi-topic comparisons. The implementation demonstrates best practices for integrating 
local LLMs with web APIs, including proper HttpClient management, cancellation support, and 
structured prompt engineering to generate well-formatted research summaries with citations.

The solution addresses common challenges like network timeouts through customizable HttpClient 
configurations while maintaining flexibility for different research scenarios. The agent 
architecture separates concerns between web search operations and LLM processing, with clear 
prompt templates ensuring consistent output formatting. Error handling covers both research 
failures and timeout scenarios, providing graceful degradation when operations exceed time 
limits or encounter network issues. This implementation serves as a practical example of 
building research automation tools using local LLMs with production-ready reliability features.

### Memory Store & Embeddings - Activity Support

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

### Handlebars Templating - Show Handlebars basic and complex code samples

This code demonstrates how to integrate **Microsoft Semantic Kernel (SK)** with **Handlebars templating** in order to create dynamic, data-driven prompts for large language models (LLMs). It uses **Ollama** to run the `llama3` model locally (`http://localhost:11434`). Two main examples are provided:

1. **BasicTemplate**
   - Sets up the kernel with Ollama as the completion backend.
   - Uses a Handlebars template to showcase **variable injection**, **loops with indices**, and **conditional rendering**.
   - Registers a **custom helper** (`equal`) to evaluate conditions within the template.
   - Injects runtime arguments (name, date, tasks, priority) and renders an adaptive output.

2. **ComplexTemplate**
   - Introduces a **UserPlugin** to fetch user profiles dynamically from SK.
   - Demonstrates **partials** (`commonFooter`) for reusable template sections.
   - Uses multiple custom helpers (`gt`, `includes`) to apply conditional logic and personalize the output.
   - Builds a tailored "Weekly Learning Report" that adapts recommendations based on user skill level and interests, and concludes with a standard footer.

Together, these functions illustrate how templating and SK plugins can combine to create personalized, maintainable, and reusable prompts that adapt to user context while keeping the prompt logic clean and testable.

## References and Links

https://learn.microsoft.com/en-us/semantic-kernel/overview/