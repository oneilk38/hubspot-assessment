using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace HubSpot.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var getEndpoint = "/dataset"; 
            var postEndpoint = "/result"; 
            
            var httpHandler = new HttpHandler("0b06bedc60f23c486966218e84b6", "https://candidate.hubteam.com/candidateTest/v3/problem");
            var inbox = new Inbox(httpHandler);
            
            var result = inbox.GetConversations(getEndpoint, postEndpoint).Result;
            
            var body = result.Content.ReadAsStringAsync().Result;
            
            Console.WriteLine($"POST request was {(result.IsSuccessStatusCode ? "Accepted" : "Rejected")}, {body}");
        }
    }
}