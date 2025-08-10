# Auto Function Calling
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

## Travel Planner Code Overview

This solution demonstrates an AI-powered travel assistant that automatically calls native functions 
to gather trip information in response to natural language requests. Key components:

### 1. Core Setup
```
   - Uses OpenAI's chat model (gpt-3.5-turbo/gpt-4 or ollama-3)
   - Configures AutoInvokeKernelFunctions for hands-free function execution
```
### 2. Travel Plugin
```
 - Four Decorated Functions:
    - CheckFlights(origin, destination, date)
    - FindHotels(location, checkIn, checkOut, guests)
    - GetWeather(location)
    - ConvertCurrency(fromCurrency, toCurrency, amount)
    - Each tagged with [KernelFunction] and rich descriptions for AI understanding
```
### 3. Auto Function Calling Workflow
```
    - User submits a natural language request (e.g., "Plan my trip to Paris...")
    - AI autonomously:
      
      1. Identifies needed functions
      2. Extracts parameters (dates, locations, etc.) from text
      3. Executes functions in logical order
      4. Weaves results into a coherent response
```
4. Streaming Output
```
  - Displays real-time progress with:
  - Thought process ("Searching for flights...")
  - Function call results
  - Final compiled itinerary
```
5. Sample Output

```
Assistant: Found 3 flights from NYC to Paris on 6/15. 
[Called CheckFlights] 
5 hotels available for your dates. 
[Called FindHotels] 
Weather: Sunny, 25°C. 
[Called GetWeather] 
1000 USD = 920 EUR. 
[Called ConvertCurrency]
```

## Key Strengths

- No Manual Orchestration – AI handles function chaining

- Context-Aware – Understands dates, locations, and conversions

- Extensible – Add functions for rentals, attractions, etc.

- Real-World Ready – Demonstrates API integrations (weather, bookings, finance)

This pattern is ideal for chatbots, copilots, or automation tools requiring dynamic backend interactions.