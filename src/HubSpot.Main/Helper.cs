using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HubSpot.Main
{


    public class Attendee
    {
        public string email { get; set; }
        public string country { get; set; }
        public string[] dates { get; set; }

        public Attendee(string e, string c, string[] d)
        {
            email = e;
            country = c;
            dates = d; 
        }
    }
    
    // Helper Functions 
    public class Helper
    {
        public static DateTime ToDateTime(string dateStr)
        {
          var date = dateStr.Replace("-", "");
          return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
        
        /*
         group by country 
            var attendee1 = new Attendee("kevin@hubspot.com", "IRL", new string[] {"2020-01-03", "2020-02-02"});
            var attendee2 = new Attendee("eoin@hubspot.com", "USA", new string[] {"2020-04-10", "2020-05-07"});
            var attendee3 = new Attendee("jason@hubspot.com", "IRL", new string[] {"2020-06-10", "2020-05-07"});
            var attendees = new Attendee[] {attendee1, attendee2, attendee3};

            
            var d = 
                attendees
                    .GroupBy(attendee => attendee.country)
                    .ToDictionary(
                group  => group.Key, 
             group => group.ToList());
         */

        public static DateTime[] ToSortedDateTimes(string[] dates)
        {
            return dates
                .Select(ToDateTime)
                .OrderBy(date => date)
                .ToArray();
        }
        
        /*
         
         Group by country 
         for each grouping of people by country, 
            get unique dates and sort them 
            find pairs of sequential days 
                if some exist
                    for each pair 
                        for each customer in country 
                            if customer can attend date1 OR date2, attendees++, add email to payload
                            else nothing 
                        
                if none exist return null response for country 
         
         */ 


    }
}