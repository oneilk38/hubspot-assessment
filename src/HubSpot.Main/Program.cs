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
            var getEndpoint = "/posts"; 
            var postEndpoint = "/posts"; 
            
            var httpHandler = new HttpHandler("bd18103ee5a98f08cb8872c817485093", "https://jsonplaceholder.typicode.com/");
            var solution = new Solution(httpHandler);
            
            var result = solution.Run(getEndpoint, postEndpoint).Result;
            
            var body = result.Content.ReadAsStringAsync().Result;
            
            Console.WriteLine($"POST request was {(result.IsSuccessStatusCode ? "Accepted" : "Rejected")}, {body}");
        }
    }
}