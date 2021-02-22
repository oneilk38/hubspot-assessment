using System.Collections.Generic;

namespace HubSpot.Main
{
    // The response from the GET request will be deserialised to this type 
    public class HubspotInput
    {
        public string @base { get; set; } 
        public string date { get; set; } 
    }
    
    // The output type that we will POST to hubspot API 
    public class HubspotOutput
    {
        public string someOutputField { get; set; }
    } 
}