using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.AutoFunctionCalling;

public class TravelPlugin
{
   [KernelFunction, Description("Check available flights between two locations on a specific date")]
   public string CheckFlights(
       [Description("Departure city")] string origin,
       [Description("Destination city")] string destination,
       [Description("Travel date")] string date)
   {
      return $"Flight from {origin} to {destination} on {date}: 3 options found";
   }

   [KernelFunction, Description("Find hotel options in a specific location")]
   public string FindHotels(
       [Description("City/location")] string location,
       [Description("Check-in date")] string checkIn,
       [Description("Check-out date")] string checkOut,
       [Description("Number of guests")] int guests)
   {
      return $"Hotels in {location} from {checkIn} to {checkOut} for {guests}: 5 options found";
   }

   [KernelFunction, Description("Get current weather conditions")]
   public string GetWeather([Description("City/location")] string location)
   {
      return $"Weather in {location}: Sunny, 25°C";
   }

   [KernelFunction, Description("Convert between currencies")]
   public string ConvertCurrency(
       [Description("Source currency")] string fromCurrency,
       [Description("Target currency")] string toCurrency,
       [Description("Amount to convert")] decimal amount)
   {
      return $"{amount} {fromCurrency} = {amount * 0.92m} {toCurrency} (current rate)";
   }
}
