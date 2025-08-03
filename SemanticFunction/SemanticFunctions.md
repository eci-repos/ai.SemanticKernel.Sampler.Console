# Semantic Functions

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

```
graph TD
  A[Template Text] --> B(PromptTemplateFactory)
  C[Template Config] --> B
  B --> D[IPromptTemplate]
  D --> E[RenderAsync with Variables]
  E --> F[Rendered Prompt]
```

## Code Sample

Key components and features demonstrated in the Semantic Kernel code sample:

### 1. Prompt Template Config
The configuration layer that defines the semantic function's metadata and parameters:

``` 
public class PromptTemplateConfig {
    public string Name { get; set; } // Function identifier
    public string Description { get; set; } // Human-readable purpose
    public List<InputParameter> InputParameters { get; } // Parameter definitions
    
    public class InputParameter {
        public string Name { get; set; } // Variable name (e.g., "product")
        public string Description { get; set; } // Parameter documentation
        public string DefaultValue { get; set; } // Fallback value
        public bool IsRequired { get; set; } // Validation flag
    }
}
```

#### Key Features:

- Declarative parameter specification
- Built-in validation rules
- Documentation-ready metadata
- Default value management

### 2. Kernel Extension
The infrastructure layer that bridges configuration with execution:

```
public static KernelFunction CreateFunctionFromPrompt(
    this Kernel kernel,
    string promptTemplate,
    PromptTemplateConfig config)
{
    return KernelFunctionFactory.CreateFromPrompt(
        promptTemplate,
        description: config.Description,
        executionSettings: new OpenAIPromptExecutionSettings {
            Temperature = 0.7,
            MaxTokens = 500
        });
}
```

#### Key Features:

- Extension method pattern for clean integration
- Converts config objects to runtime components
- Centralizes prompt template registration
- Abstracts AI service configuration

### 3. Semantic Function Implementation
The operational layer that executes the AI task:

```
string promptTemplate = """
    Generate a {{$length}} description for {{$product}}.
    Tone: {{$tone}}.
    """;

var function = kernel.CreateFunctionFromPrompt(promptTemplate, config);

var result = await function.InvokeAsync(kernel, new KernelArguments {
    { "product", "self-heating mug" },
    { "tone", "enthusiastic" }
});
```

#### Key Features:

- Handlebars-style templating ({{$var}})
- Dynamic variable injection
- Type-safe argument passing
- Async execution model
- Integrated prompt engineering

#### Architectural Benefits:

- Separation of Concerns - Config, infrastructure, and execution layers are distinct
- Reusability - Functions can be registered and shared across solutions
- Discoverability - Rich metadata enables automated documentation
- Portability - Templates work across different AI backends
- Testability - Components can be unit tested in isolation

The implementation showcases Semantic Kernel's core value proposition: 
transforming ad-hoc prompt engineering into structured, maintainable software components.

