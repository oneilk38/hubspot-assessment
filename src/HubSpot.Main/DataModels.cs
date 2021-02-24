using System.Collections.Generic;

namespace HubSpot.Main
{
    // The response from the GET request will be deserialised to this type 

    public class HubspotInput
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
    
    // The output type that we will POST to hubspot API 
    public class HubspotOutput
    {
        public int userId { get; set; }
        public string title { get; set; }
        public string body { get; set; }

        public HubspotOutput()
        {
            userId = 999;
            title = "Book title";
            body = "A B C D E F G H .....";
        }
    } 
}