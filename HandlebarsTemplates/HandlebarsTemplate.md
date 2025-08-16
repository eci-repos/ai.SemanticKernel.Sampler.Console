# Handlebars Templating

Handlebars is a popular templating language that Microsoft Semantic Kernel has integrated 
to provide powerful prompt templating capabilities. This integration allows you to create 
dynamic prompts with logic, conditionals, and loops.

## Key Features of Handlebars in Semantic Kernel

1. **Variables**: Insert dynamic values into prompts
2. **Conditionals**: Add logic with if/else statements
3. **Loops**: Iterate over collections with each
4. **Helpers**: Custom functions for complex operations
5. **Partials**: Reusable template components



# Handlebars Feature Guid (v1.0.1)

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