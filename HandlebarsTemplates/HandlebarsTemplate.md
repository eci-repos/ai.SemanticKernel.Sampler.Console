# Handlebars Templating

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

> **Note:** In `ComplexTemplate`, the template expects `userId` but the invocation currently passes `userid` (lowercase *d*). This should be aligned to ensure proper plugin binding.

---

## Goal of the Approach

- **Separate presentation from orchestration:** Templates hold content and structure while the kernel manages model calls and plugin execution.
- **Enable dynamic, data-driven prompts:** Fetch real data (e.g., user profiles) at runtime and inject it into structured templates.
- **Promote reusability and consistency:** Common elements (footers, helpers) can be reused across multiple prompts.

---

## Benefits of Handlebars Templating in Semantic Kernel

- **Readable and maintainable prompts:** Templates are easy to read, edit, and manage without cluttering application code.
- **Reduced complexity:** Eliminates fragile string concatenation by embedding logic (loops, conditions, formatting) into the template.
- **Reusability:** Partials and helpers allow developers to reuse common sections and logic across multiple templates.
- **Integration with SK plugins:** Templates can call kernel plugins for live data, creating personalized and context-aware model prompts.
- **Flexible deployment:** Using Ollama locally makes iteration fast, while enabling later migration to cloud-hosted or enterprise-scale models with minimal code changes.

## Key Features of Handlebars in Semantic Kernel

1. **Variables**: Insert dynamic values into prompts
2. **Conditionals**: Add logic with if/else statements
3. **Loops**: Iterate over collections with each
4. **Helpers**: Custom functions for complex operations
5. **Partials**: Reusable template components

# Handlebars Feature Guide (based on v1.0.1)

## **1. Core Syntax**  
### **Variables**  
```handlebars
Hello, {{name}}! Today is {{date}}.
```
- **Usage**: Injects dynamic values from `KernelArguments`.  
- **Null Handling**: Throws if undefined (use `{{#if name}}`).  

### **Conditionals**  
```handlebars
{{#if (equal status "completed")}}
  Done!
{{else}}
  In progress...
{{/if}}
```
- **Supported Operators**:  
  `equal`, `not-equal`, `gt`, `lt`, `or`, `and`  
- **Truthy/Falsy**: Empty collections/strings are falsy.  

---

## **2. Loops & Iteration**  
### **Basic `each` Loop**  
```handlebars
{{#each items}}
  - {{@index}}: {{this}}
{{/each}}
```
- **Special Variables**:  
  `@index` (0-based), `@index1` (1-based), `@first`, `@last`  

### **Nested Loops**  
```handlebars
{{#each users}}
  ## {{name}}'s Tasks:
  {{#each tasks}}
    - {{this}} ({{@../index}})
  {{/each}}
{{/each}}
```
- **Parent Context**: Use `@../` to reference outer scope.  

---

## **3. Helpers (Built-in & Custom)**  
### **Built-in Helpers**  
| Helper | Example | Purpose |
|--------|---------|---------|
| `json` | `{{json data}}` | Serializes objects to JSON |
| `concat` | `{{concat str1 str2}}` | Joins strings |
| `default` | `{{default val "N/A"}}` | Fallback values |

### **Custom Helpers**  
```csharp
// Define a helper class
public class TextHelpers
{
    [KernelFunction]
    public string Slugify(string input) 
        => Regex.Replace(input, @"[^a-z0-9]", "-").ToLower();
}

// Register
kernel.ImportPluginFromObject(new TextHelpers(), "text");

// Usage: {{text.slugify "Hello World!"}} → "hello-world"
```

---

## **4. Advanced Features**  
### **Partials (Reusable Templates)**  
1. Save a partial in `./Prompts/_signature.handlebars`:  
   ```text
   Best regards,
   {{author}}
   ```
2. Reference it:  
   ```handlebars
   {{message}}
   {{> _signature}}
   ```

### **Inline Expressions**  
```handlebars
{{#set user = (kernel "UserPlugin.GetUser" userId=id)}}
Welcome back, {{user.Name}}!
```

### **JSON Manipulation**  
```handlebars
{{#json}}
{
  "request": "summarize",
  "items": [
    {{#each items}}
    "{{this}}"{{#unless @last}},{{/unless}}
    {{/each}}
  ]
}
{{/json}}
```

---

## **5. Semantic Kernel Integration**  
### **Prompt Configuration**  
```csharp
var promptConfig = new PromptTemplateConfig
{
    Template = handlebarsTemplate,
    TemplateFormat = "handlebars", // Required
    InputVariables = 
    {
        new() { Name = "topics", Description = "List of topics" }
    }
};

var function = kernel.CreateFunctionFromPrompt(promptConfig);
```

### **Argument Binding**  
```csharp
var args = new KernelArguments
{
    { "topics", new List<string> { "AI", "ML" } },
    { "userLevel", "advanced" }
};
```

---

## **6. Limitations & Workarounds**  
| Limitation | Workaround |
|------------|------------|
| No IntelliSense | Use `[Description]` attributes in helpers |
| No pre-compilation | Cache `KernelFunction` objects |
| Strict error handling | Wrap variables in `{{#if}}` checks |
| Limited debug output | Log `KernelArguments` before invocation |

---

## **7. Best Practices**  
1. **Modularize Templates**:  
   ```text
   Prompts/
   ├── main.handlebars
   └── Partials/
       ├── _header.handlebars
       └── _footer.handlebars
   ```

2. **Defensive Templating**:  
   ```handlebars
   {{#if user}}
     {{user.Name}}
   {{else}}
     Guest
   {{/if}}
   ```

3. **Type Safety**:  
   ```csharp
   // Strongly-typed arguments
   var args = new KernelArguments
   {
       { "user", new User("John") } // Class properties map to template vars
   };
   ```

---

## **8. Example: Complex Workflow**  
```handlebars
{{! Fetch user data dynamically }}
{{#set user = (kernel "UserPlugin.GetUser" userId=userId)}}

{{#if (gt user.Score 100)}}
  🎉 **Premium Summary for {{user.Name}}**:
  {{> _premiumContent}}
{{else}}
  **Basic Summary**:
  {{> _basicContent}}
{{/if}}

{{> _footer}}
```

---

## **Key Takeaways**  
- **Handlebars is runtime-evaluated** – No compile-time checks.  
- **Helpers extend functionality** – Add custom logic via C#.  
- **Partials promote reusability** – Ideal for headers/footers.  
- **Semantic Kernel tightly integrates** – Use `kernel` helper for dynamic data.  

For the latest updates, refer to the [official docs](https://learn.microsoft.com/en-us/semantic-kernel/prompts/handlebars-templates).