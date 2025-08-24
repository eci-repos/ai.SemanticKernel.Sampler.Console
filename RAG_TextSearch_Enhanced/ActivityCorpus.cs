using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -------------------------------------------------------------------------------------------------
namespace ai.SemanticKernel.Sampler.Console.RAG_TextSearch_Enhanced;

/// <summary>
/// Represents an activity corpus with associated code and base information.
/// </summary>
public class ActivityCorpus
{
   public string Code { get; set; }
   public string Base { get; set; }

   /// <summary>
   /// Retrieves a sample collection of activity corpus objects.
   /// </summary>
   /// <remarks>This method is intended for use in scenarios where a predefined set of activity data 
   /// is needed, such as testing or demonstration purposes. The returned data is static and does 
   /// not reflect real-world or dynamic
   /// content.</remarks>
   /// <returns>An array of <see cref="ActivityCorpus"/> objects representing a predefined sample 
   /// dataset. The array will contain
   /// at least one item.</returns>
   public static ActivityCorpus[] GetSampleCorpus()
   {
      return new ActivityCorpus[]
      {
    new ActivityCorpus{
        Code="ACT-101",
        Base="Community Yoga Class\n\nOverview: A beginner-friendly yoga session focused on flexibility and stress relief.\n\nRegistration: Register online at /register/act-101. Capacity 25. Drop-ins allowed if spots remain. Fee: $12.\n\nParticipants: Ages 16+. Bring a mat; loaners available.\n\nLocation: Willow Center, Room A, 1st floor. Check in at the front desk."
    },
    new ActivityCorpus{
        Code="ACT-202",
        Base="Intro to Pickleball\n\nOverview: Learn rules, scoring, and safety; then 2v2 scrimmages.\n\nRegistration: Required; closes 24 hours prior. Equipment provided. Fee: $15.\n\nParticipants: Ages 12+. Max 16 players per timeslot.\n\nLocation: Riverside Gym Court 2. Arrive 10 minutes early."
    },
    new ActivityCorpus{
        Code="ACT-303",
        Base="Trail Cleanup Day\n\nOverview: Volunteer cleanup on the Lakeside Loop.\n\nRegistration: Free; waivers required. Gloves and bags provided.\n\nParticipants: All ages; minors with guardian. Community service hours available.\n\nLocation: Meet at Lakeside Trailhead kiosk; carpool recommended."
    }
      };
   }

}
