# Prompt Template Loader

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

```
sequenceDiagram
    participant App
    participant Factory
    participant FileSystem
    
    App->>Factory: LoadAsync("ProductDescription")
    Factory->>FileSystem: Read skprompt.txt
    Factory->>FileSystem: Read config.json
    FileSystem-->>Factory: Template text
    FileSystem-->>Factory: Config data
    Factory->>Factory: Create IPromptTemplate
    Factory-->>App: Ready-to-use template
```

## Understanding ```PromptTemplateFactory.Load```

This method loads templates from a directory, automatically parsing template files and their 
configuration, details follow. 

